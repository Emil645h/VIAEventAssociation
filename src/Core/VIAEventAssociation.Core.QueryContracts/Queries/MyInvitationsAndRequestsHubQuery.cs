using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.Queries;

public record MyInvitationsAndRequestsHubQuery(
    string GuestId
) : IQuery<MyInvitationsAndRequestsHubAnswer>;

public record MyInvitationsAndRequestsHubAnswer(
    int PendingInvitationsCount,
    int PendingRequestsCount,
    int RecentActivityCount,
    List<PendingInvitationInfo> PendingInvitations,
    List<PendingRequestInfo> PendingRequests,
    List<RecentActivityInfo> RecentActivities
);

public record PendingInvitationInfo(
    string InvitationId,
    string EventId,
    string EventTitle,
    string EventDate,
    string EventTime,
    int CurrentAttendees,
    int MaxAttendees,
    bool IsUpcoming
);

public record PendingRequestInfo(
    string RequestId,
    string EventId,
    string EventTitle,
    string EventDate,
    string EventTime,
    string RequestedDate,
    string Status
);

public record RecentActivityInfo(
    string ActivityType, // "invitation", "request"
    string EventTitle,
    string Status, // "Accepted", "Declined", "Approved", "Rejected"
    string ActivityDate,
    string Description // Formatted description for display
);