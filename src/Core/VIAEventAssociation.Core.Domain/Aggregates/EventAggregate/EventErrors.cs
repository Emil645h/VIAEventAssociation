using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;

public static class EventErrors
{
    public static class EventId
    {
        private const string EventIdCode = "Event.Id";

        public static ResultError IsEmpty = new(EventIdCode, "Event Id cannot be empty.");
    }

    public static class EventTitle
    {
        private const string EventTitleCode = "Event.Title";
        
        public static readonly ResultError TitleIsEmpty = new(EventTitleCode, "Event title cannot be empty.");
        public static readonly ResultError TitleIsNull = new(EventTitleCode, "Event title cannot be null.");
        public static readonly ResultError InvalidTitleCharacterLimit = new(EventTitleCode, "Event title must be between 3 and 75 characters.");
        public static readonly ResultError InvalidEventStatus = new(EventTitleCode, "Event status is either active or cancelled.");
        
    }
}