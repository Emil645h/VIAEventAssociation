using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;

public static class EventErrors
{
    public static class EventId
    {
        private const string EventIdCode = "Event.Id";

        public static ResultError IsEmpty = new(EventIdCode, "Event Id cannot be empty.");
    }
}