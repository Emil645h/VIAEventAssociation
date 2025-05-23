using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.Queries;

public record GuestProfilePageQuery(string GuestId) : IQuery<GuestProfilePageAnswer>;

public record GuestProfilePageAnswer(
    string GuestId,
    string FirstName, 
    string LastName,
    string Email,
    string ProfilePictureUrl,
    int UpcomingEventsCount,
    List<UpcomingEventInfo> UpcomingEvents,
    List<PastEventInfo> PastEvents,
    int PendingInvitationsCount);
    
public record UpcomingEventInfo(
    string EventId,
    string Title,
    int AttendeesCount,
    string Date, // This is formatted nicely
    string StartTime);
    
public record PastEventInfo(
    string EventId,
    string Title);