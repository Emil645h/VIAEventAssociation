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

    public static class EventReadyStatus
    {
        private const string EventStatusCode = "Event.Status.Ready";
        
        public static readonly ResultError TitleIsDefault = new(
            EventStatusCode, "The event title must be changed from the default value.");
            
        public static readonly ResultError TimesNotSet = new(
            EventStatusCode, "Event times must be set before the event can be made ready.");
            
        public static readonly ResultError VisibilityNotSet = new(
            EventStatusCode, "Event visibility must be set before the event can be made ready.");
            
        public static readonly ResultError MaxGuestsInvalid = new(
            EventStatusCode, "Maximum guests must be between 5 and 50 before the event can be made ready.");
            
        public static readonly ResultError StartTimeInPast = new(
            EventStatusCode, "An event in the past cannot be made ready.");
            
        public static readonly ResultError CancelledEventCannotBeReadied = new(
            EventStatusCode, "A cancelled event cannot be readied.");
            
        public static readonly ResultError NotInDraftStatus = new(
            EventStatusCode, "Only events in draft status can be made ready.");
    }

    public static class EventActiveStatus
    {
        private const string EventActiveStatusCode = "Event.Status.Active";
        
        public static readonly ResultError CancelledEventCannotBeActivated = new(
            EventActiveStatusCode, "A cancelled event cannot be activated.");
        
        public static readonly ResultError TimesNotSet = new(
            EventActiveStatusCode, "Event times must be set before the event can be made ready.");
        
        public static readonly ResultError StartTimeInPast = new(
            EventActiveStatusCode, "An event in the past cannot be made ready.");
        
        public static readonly ResultError TitleIsDefault = new(
            EventActiveStatusCode, "The event title must be changed from the default value.");
    }

    public static class EventMaxGuests
    {
        private const string EventMaxGuestsCode = "Event.MaxGuests";
        
        public static readonly ResultError IsNull = new(EventMaxGuestsCode, "The events max guest amount cannot be null.");

        public static readonly ResultError TooSmall = new(EventMaxGuestsCode,
            "The maximum number of guests cannot be less than 5.");
        public static readonly ResultError TooLarge = new(EventMaxGuestsCode,
            "The maximum number of guests cannot be larger than 50.");
        public static readonly ResultError CannotBeReduced = new(EventMaxGuestsCode,
            "The maximum number of guests of an active event cannot be reduced (only increased).");
        public static readonly ResultError CancelledEventCannotBeModified = new(EventMaxGuestsCode,
            "Cancelled events cannot be modified.");
        public static readonly ResultError ExceedsLocationCapacity = new(EventMaxGuestsCode,
            "The maximum number of guests cannot exceed the location's maximum capacity.");
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

    public static class Invitation
    {
        private const string InvitationCode = "Event.Invitation";
        
        public static readonly ResultError InvitationNotFound = new(
            InvitationCode, "The guest is not invited to this event.");
            
        public static readonly ResultError EventFull = new(
            InvitationCode, "The event is full.");
            
        public static readonly ResultError EventCancelled = new(
            InvitationCode, "Cancelled events cannot be joined.");
            
        public static readonly ResultError EventNotActive = new(
            InvitationCode, "The event cannot yet be joined.");
            
        public static readonly ResultError EventInPast = new(
            InvitationCode, "Past events cannot be joined.");
    }
}