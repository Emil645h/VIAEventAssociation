using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events;

public static class EventErrors
{
    public static class EventId
    {
        private const string EventIdCode = "Event.Id";
        
        public static ResultError IsEmpty = new(EventIdCode, "Event ID cannot be empty.");
    }
}