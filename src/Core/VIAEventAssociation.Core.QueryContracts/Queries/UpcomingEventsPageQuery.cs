using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.Queries;

public record UpcomingEventsPageQuery(
    string? SearchText,
    int PageNumber,
    int PageSize
) : IQuery<UpcomingEventsPageAnswer>;

public record UpcomingEventsPageAnswer(
    List<UpcomingEventItem> Events,
    int CurrentPage,
    int TotalPages,
    int TotalEvents
);

public record UpcomingEventItem(
    string EventId,
    string Title,
    string Description,
    string Date,
    string StartTime,
    int CurrentAttendees,
    int MaxAttendees,
    bool IsPublic,
    bool IsPrivate
);