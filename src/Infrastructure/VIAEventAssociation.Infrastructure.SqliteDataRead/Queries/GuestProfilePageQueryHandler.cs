using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Common;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;

public class GuestProfilePageQueryHandler : IQueryHandler<GuestProfilePageQuery, GuestProfilePageAnswer>
{
    private readonly VeadatabaseProductionContext _context;
    private readonly ICurrentTime _currentTime;

    public GuestProfilePageQueryHandler(VeadatabaseProductionContext context, ICurrentTime currentTime)
    {
        _context = context;
        _currentTime = currentTime;
    }

    public async Task<GuestProfilePageAnswer> HandleAsync(GuestProfilePageQuery query)
    {
        // Get current time to compare
        string currentTime = _currentTime.GetCurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");
        
        // Get guest basic info
        var guest = await _context.Guests
            .Where(g => g.Id == query.GuestId)
            .Select(g => new
            {
                g.Id,
                g.FirstName,
                g.LastName,
                g.Email,
                g.ProfilePictureUrl
            }).SingleOrDefaultAsync();

        if (guest == null)
        {
            throw new InvalidOperationException($"Guest with ID {query.GuestId} not found");
        }

        // Get upcoming events for the guest
        var upcomingEvents = await _context.GuestListEntries
            .Where(gle => gle.GuestId == query.GuestId)
            .Join(_context.GuestLists, gle => gle.GuestListId, gl => gl.Id, (gle, gl) => gl.EventId)
            .Join(_context.Events, eventId => eventId, e => e.Id, (eventId, e) => e)
            .Where(e => e.StartTime != null && e.StartTime.CompareTo(currentTime) > 0)
            .OrderBy(e => e.StartTime)
            .Select(e => new
            {
                EventId = e.Id,
                e.Title,
                e.StartTime,
                AttendessCount = _context.GuestListEntries
                    .Count(gle => _context.GuestLists
                        .Where(gl => gl.EventId == e.Id)
                        .Select(gl => gl.Id)
                        .Contains(gle.GuestListId))
            }).ToListAsync();
        
        // Get past events for the guest (limited to 5, newest first)
        var pastEvents = await _context.GuestListEntries
            .Where(gle => gle.GuestId == query.GuestId)
            .Join(_context.GuestLists, gle => gle.GuestListId, gl => gl.Id, (gle, gl) => gl.EventId)
            .Join(_context.Events, eventId => eventId, e => e.Id, (eventId, e) => e)
            .Where(e => e.StartTime != null && e.StartTime.CompareTo(currentTime) < 0)
            .OrderByDescending(e => e.StartTime)
            .Take(5)
            .Select(e => new
            {
                EventId = e.Id,
                e.Title
            }).ToListAsync();

        // Get pending invitations count
        var pendingInvitationsCount = await _context.Invites
            .Where(i => i.AssignedGuestId == query.GuestId && i.Status == "Pending")
            .CountAsync();

        // Transform upcoming events data
        var upcomingEventInfos = upcomingEvents.Select(e => new UpcomingEventInfo(
            e.EventId,
            e.Title,
            e.AttendessCount,
            DateTimeFormatHelper.FormatDateForProfile(e.StartTime),
            DateTimeFormatHelper.FormatTime(e.StartTime)
        )).ToList();
        
        // Transform past events data
        var pastEventInfos = pastEvents.Select(e => new PastEventInfo(
            e.EventId,
            e.Title
        )).ToList();

        return new GuestProfilePageAnswer(
            guest.Id,
            guest.FirstName,
            guest.LastName,
            guest.Email,
            guest.ProfilePictureUrl,
            upcomingEvents.Count,
            upcomingEventInfos,
            pastEventInfos,
            pendingInvitationsCount
        );
    }
}