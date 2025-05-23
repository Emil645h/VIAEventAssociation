using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Common;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;

public class ViewSingleEventQueryHandler : IQueryHandler<ViewSingleEventQuery, ViewSingleEventAnswer>
{
    private readonly VeadatabaseProductionContext _context;

    public ViewSingleEventQueryHandler(VeadatabaseProductionContext context)
    {
        _context = context;
    }

    public async Task<ViewSingleEventAnswer> HandleAsync(ViewSingleEventQuery query)
    {
        // Get event basic information
        var eventInfo = await _context.Events
            .Where(e => e.Id == query.EventId)
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Description,
                e.StartTime,
                e.EndTime,
                e.MaxGuests,
                e.Visibility
            })
            .SingleOrDefaultAsync();

        if (eventInfo == null)
        {
            throw new InvalidOperationException($"Event with ID {query.EventId} not found");
        }

        // Get all attendees for this event from multiple sources
        var allAttendeeIds = await GetAllAttendeeIds(query.EventId);
        
        // Remove duplicates and get total count
        var uniqueAttendeeIds = allAttendeeIds.Distinct().ToList();
        var totalGuests = uniqueAttendeeIds.Count;
        var totalRows = (int)Math.Ceiling((double)totalGuests / query.GuestsPerRow);

        // Calculate pagination based on rows
        var guestsToSkip = query.RowOffset * query.GuestsPerRow;
        var guestsToTake = query.RowsToShow * query.GuestsPerRow;

        // Apply row-based pagination to guest IDs
        var pagedAttendeeIds = uniqueAttendeeIds
            .Skip(guestsToSkip)
            .Take(guestsToTake)
            .ToList();

        // Get guest details for the current page
        var guests = await _context.Guests
            .Where(g => pagedAttendeeIds.Contains(g.Id))
            .OrderBy(g => g.FirstName)
            .ThenBy(g => g.LastName)
            .Select(g => new EventGuestInfo(
                g.Id,
                g.FirstName,
                g.LastName,
                g.FirstName + " " + g.LastName,
                g.ProfilePictureUrl
            ))
            .ToListAsync();

        // Maintain the order from pagedAttendeeIds
        var orderedGuests = pagedAttendeeIds
            .Select(id => guests.FirstOrDefault(g => g.GuestId == id))
            .Where(g => g != null)
            .ToList()!;

        return new ViewSingleEventAnswer(
            eventInfo.Id,
            eventInfo.Title,
            eventInfo.Description,
            "C05.19", // Location - you might want to add this to your Event table
            DateTimeFormatHelper.FormatDate(eventInfo.StartTime),
            DateTimeFormatHelper.FormatTime(eventInfo.StartTime),
            DateTimeFormatHelper.FormatTime(eventInfo.EndTime),
            eventInfo.Visibility.Equals("Public", StringComparison.OrdinalIgnoreCase),
            eventInfo.Visibility,
            totalGuests,
            eventInfo.MaxGuests,
            orderedGuests,
            query.RowOffset,
            totalGuests,
            totalRows,
            query.RowOffset + query.RowsToShow < totalRows
        );
    }

    private async Task<List<string>> GetAllAttendeeIds(string eventId)
    {
        var attendeeIds = new List<string>();

        // 1. Get participants (from GuestListEntries)
        var participants = await _context.GuestListEntries
            .Where(gle => _context.GuestLists
                .Where(gl => gl.EventId == eventId)
                .Select(gl => gl.Id)
                .Contains(gle.GuestListId))
            .Select(gle => gle.GuestId)
            .ToListAsync();

        attendeeIds.AddRange(participants);

        // 2. Get guests who accepted invitations
        var acceptedInvitees = await _context.Invites
            .Where(i => i.EventId == eventId && i.Status == "Accepted")
            .Select(i => i.AssignedGuestId)
            .Where(id => id != null)
            .ToListAsync();

        attendeeIds.AddRange(acceptedInvitees!);

        // 3. Get guests whose join requests were accepted
        var acceptedRequesters = await _context.Requests
            .Where(r => r.EventId == eventId && r.Status == "Accepted")
            .Select(r => r.AssignedGuestId)
            .Where(id => id != null)
            .ToListAsync();

        attendeeIds.AddRange(acceptedRequesters!);

        return attendeeIds;
    }
}