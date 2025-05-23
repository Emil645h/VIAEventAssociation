using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.Queries;

public record EventsEditingOverviewQuery() : IQuery<EventsEditingOverviewAnswer>;

public record EventsEditingOverviewAnswer(
    List<EventSummary> DraftEvents,
    List<EventSummary> ReadyEvents,
    List<EventSummary> CancelledEvents,
    int TotalDrafts,
    int TotalReady,
    int TotalCancelled
);

public record EventSummary(
    string EventId,
    string Title
);