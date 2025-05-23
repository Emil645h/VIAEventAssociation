using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Common;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;

public class MyInvitationsAndRequestsHubQueryHandler : IQueryHandler<MyInvitationsAndRequestsHubQuery, MyInvitationsAndRequestsHubAnswer>
{
    private readonly VeadatabaseProductionContext _context;
    private readonly ICurrentTime _currentTime;

    public MyInvitationsAndRequestsHubQueryHandler(VeadatabaseProductionContext context, ICurrentTime currentTime)
    {
        _context = context;
        _currentTime = currentTime;
    }

    public async Task<MyInvitationsAndRequestsHubAnswer> HandleAsync(MyInvitationsAndRequestsHubQuery query)
    {
        var currentTime = _currentTime.GetCurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");

        // Get pending invitations
        var pendingInvitations = await GetPendingInvitations(query.GuestId, currentTime);
        
        // Get pending join requests
        var pendingRequests = await GetPendingRequests(query.GuestId, currentTime);
        
        // Get recent activity (last 5 activities)
        var recentActivities = await GetRecentActivity(query.GuestId);

        return new MyInvitationsAndRequestsHubAnswer(
            pendingInvitations.Count,
            pendingRequests.Count,
            recentActivities.Count,
            pendingInvitations,
            pendingRequests,
            recentActivities
        );
    }

    private async Task<List<PendingInvitationInfo>> GetPendingInvitations(string guestId, string currentTime)
    {
        var invitationData = await _context.Invites
            .Where(i => i.AssignedGuestId == guestId && i.Status == "Pending")
            .Join(_context.Events, i => i.EventId, e => e.Id, (i, e) => new { Invite = i, Event = e })
            .Select(ie => new
            {
                InviteId = ie.Invite.Id,
                EventId = ie.Event.Id,
                ie.Event.Title,
                ie.Event.StartTime,
                ie.Event.MaxGuests,
                AttendeesCount = _context.GuestListEntries
                    .Where(gle => _context.GuestLists
                        .Where(gl => gl.EventId == ie.Event.Id)
                        .Select(gl => gl.Id)
                        .Contains(gle.GuestListId))
                    .Count()
            })
            .ToListAsync();

        return invitationData.Select(inv => new PendingInvitationInfo(
            inv.InviteId,
            inv.EventId,
            inv.Title,
            DateTimeFormatHelper.FormatDate(inv.StartTime),
            DateTimeFormatHelper.FormatTime(inv.StartTime),
            inv.AttendeesCount,
            inv.MaxGuests,
            !string.IsNullOrEmpty(inv.StartTime) && inv.StartTime.CompareTo(currentTime) > 0
        )).OrderBy(p => p.EventDate).ToList();
    }

    private async Task<List<PendingRequestInfo>> GetPendingRequests(string guestId, string currentTime)
    {
        var requestData = await _context.Requests
            .Where(r => r.AssignedGuestId == guestId && r.Status == "Pending")
            .Join(_context.Events, r => r.EventId, e => e.Id, (r, e) => new { Request = r, Event = e })
            .Select(re => new
            {
                RequestId = re.Request.Id,
                EventId = re.Event.Id,
                re.Event.Title,
                re.Event.StartTime,
                re.Request.Status,
                // Since we don't have a request date field, we'll use a placeholder
                RequestDate = currentTime
            })
            .ToListAsync();

        return requestData.Select(req => new PendingRequestInfo(
            req.RequestId,
            req.EventId,
            req.Title,
            DateTimeFormatHelper.FormatDate(req.StartTime),
            DateTimeFormatHelper.FormatTime(req.StartTime),
            DateTimeFormatHelper.FormatDate(req.RequestDate),
            req.Status
        )).OrderBy(p => p.EventDate).ToList();
    }

    private async Task<List<RecentActivityInfo>> GetRecentActivity(string guestId)
    {
        var activities = new List<RecentActivityInfo>();

        // Get recent invitations (non-pending ones for activity)
        var recentInvitations = await _context.Invites
            .Where(i => i.AssignedGuestId == guestId && i.Status != "Pending")
            .Join(_context.Events, i => i.EventId, e => e.Id, (i, e) => new { i.Status, e.Title })
            .Take(3)
            .ToListAsync();

        foreach (var invitation in recentInvitations)
        {
            var description = invitation.Status.ToLower() switch
            {
                "accepted" => $"You accepted invitation to \"{invitation.Title}\"",
                "declined" => $"You declined invitation to \"{invitation.Title}\"",
                _ => $"Invitation to \"{invitation.Title}\" is {invitation.Status.ToLower()}"
            };

            activities.Add(new RecentActivityInfo(
                "invitation",
                invitation.Title,
                invitation.Status,
                "2 days ago", // Placeholder since we don't have timestamps
                description
            ));
        }

        // Get recent requests (non-pending ones for activity)
        var recentRequests = await _context.Requests
            .Where(r => r.AssignedGuestId == guestId && r.Status != "Pending")
            .Join(_context.Events, r => r.EventId, e => e.Id, (r, e) => new { r.Status, e.Title })
            .Take(2)
            .ToListAsync();

        foreach (var request in recentRequests)
        {
            var description = request.Status.ToLower() switch
            {
                "accepted" => $"Your request to join \"{request.Title}\" was approved",
                "declined" => $"Your request to join \"{request.Title}\" was declined",
                _ => $"Your request to join \"{request.Title}\" is {request.Status.ToLower()}"
            };

            activities.Add(new RecentActivityInfo(
                "request",
                request.Title,
                request.Status,
                "3 days ago", // Placeholder since we don't have timestamps
                description
            ));
        }

        return activities.Take(5).ToList();
    }
}