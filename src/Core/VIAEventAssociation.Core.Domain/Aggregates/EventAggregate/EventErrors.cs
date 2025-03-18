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

    public static class EventVisibility
    {
        private const string EventVisibilityCode = "Event.Visibility";
        
        public static readonly ResultError CancelledEventCannotBeModified = new(EventVisibilityCode, "Cancelled events cannot be modified.");
        public static readonly ResultError ActiveEventCannotBePrivate = new(EventVisibilityCode, "Active events cannot be made private.");
    }

    public static class EventStatus
    {
        private const string EventStatusCode = "Event.Status";
        
        
    }

    public static class EventDescription
    {
        private const string EventDescriptionCode = "Event.Description";

        public static readonly ResultError InvalidCharacterLength = new(EventDescriptionCode,
            "Description cannot be less than 0 and longer than 250 characters.");

        public static readonly ResultError InvalidEventStatus =
            new(EventDescriptionCode, "Invalid operation when event is active or cancelled.");
    }
    
    public static class EventTime
    {
        private const string EventTimeCode = "Event.Time";
        
        public static readonly ResultError StartDateAfterEndDate = new(
            EventTimeCode, "The start date cannot be after the end date.");
        
        public static readonly ResultError StartTimeAfterEndTime = new(
            EventTimeCode, "The start time cannot be after the end time on the same day.");
        
        public static readonly ResultError DurationTooShort = new(
            EventTimeCode, "Event duration must be at least 1 hour.");
        
        public static readonly ResultError DurationTooLong = new(
            EventTimeCode, "Event duration cannot exceed 10 hours.");
        
        public static readonly ResultError StartTimeTooEarly = new(
            EventTimeCode, "Event cannot start before 8:00.");
        
        public static readonly ResultError EndTimeTooLate = new(
            EventTimeCode, "Event cannot end after 01:00.");
        
        public static readonly ResultError InvalidTimeSpan = new(
            EventTimeCode, "Event cannot span the invalid time period between 1:00 AM and 8:00 AM.");
        
        public static readonly ResultError StartTimeInPast = new(
            EventTimeCode, "Event cannot start in the past.");
        
        public static readonly ResultError ActiveEventCannotBeModified = new(
            EventTimeCode, "Cannot update times when event is active.");
        
        public static readonly ResultError CancelledEventCannotBeModified = new(
            EventTimeCode, "Cannot update times when event is cancelled.");
    }
}