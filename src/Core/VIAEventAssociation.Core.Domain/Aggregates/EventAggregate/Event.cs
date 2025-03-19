using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class Event : AggregateRoot<EventId>
{
    internal EventTitle title;
    internal EventDescription description;
    internal EventTime? eventTime;
    internal EventVisibility visibility;
    internal EventStatus status;
    internal EventMaxGuests maxGuests;
    internal GuestList.GuestList guestList;
    internal LocationId? locationId;
    
    private Event(EventId id, EventTitle title, EventDescription description, EventTime eventTime, EventVisibility visibility, EventStatus status, EventMaxGuests maxGuests, GuestList.GuestList guestList, LocationId locationId)
        : base(id)
    {
        (this.title, this.description, this.eventTime, this.visibility, this.status, this.maxGuests, this.guestList, this.locationId)
            = (title, description, eventTime, visibility, status, maxGuests, guestList, locationId);
    }
    
    public static Result<Event> Create(EventId id)
    {
        
        var title = EventTitle.Create("Working Title").Value;
        var description = EventDescription.Create("").Value;
        var status = EventStatus.Draft;
        var maxGuests = EventMaxGuests.Create(5).Value;
        var visibility = EventVisibility.Private;
        var guestListId = GuestListId.Create(Guid.NewGuid()).Value;
        var guestList = GuestList.GuestList.Create(guestListId).Value;
        
        return new Event(id, title, description, null, visibility, status, maxGuests, guestList, null);
    }
    
    public Result<None> UpdateTitle(EventTitle title)
    {
        if (status.Equals(EventStatus.Active) || status.Equals(EventStatus.Cancelled))
            return EventErrors.EventTitle.InvalidEventStatus;
        
        this.title = title;
        status = EventStatus.Draft;
        return new None();
    }

    public Result<None> UpdateDescription(EventDescription eventDescription)
    {
        if (status.Equals(EventStatus.Cancelled) || status.Equals(EventStatus.Active))
            return EventErrors.EventDescription.InvalidEventStatus;

        description = eventDescription;
        status = EventStatus.Draft;
        return new None();
    }

    public Result<None> UpdateTime(EventTime eventTime)
    {
        if (status.Equals(EventStatus.Active))
            return EventErrors.EventTime.ActiveEventCannotBeModified;
        
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventTime.CancelledEventCannotBeModified;
        
        this.eventTime = eventTime;
        
        if (status.Equals(EventStatus.Ready))
            status = EventStatus.Draft;
        
        return new None();
    }

    public Result<None> UpdateMaxGuests(EventMaxGuests eventMaxGuests)
    {
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventMaxGuests.CancelledEventCannotBeModified;

        if (status.Equals(EventStatus.Active) && eventMaxGuests.Value < this.maxGuests.Value)
            return EventErrors.EventMaxGuests.CannotBeReduced;

        if (locationId != null)
        {
            // TODO: Need to check if location capacity is smaller than the new maxGuest value.
            // E.g. something like:
            // var locationcapacity = GetLocationCapacity(locationId.Value);
            // if (eventMaxGuests.Value > locationCapacity)
            //     return EventErrors.EventMaxGuests.ExceedsLocationCapacity;
        }
        maxGuests = eventMaxGuests;
        
        return new None();
    }

    public Result<None> MakePublic()
    {
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventVisibility.CancelledEventCannotBeModified;
        
        visibility = EventVisibility.Public;
        return new None();
    }
    
    public Result<None> MakePrivate()
    {
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventVisibility.CancelledEventCannotBeModified;

        if (status.Equals(EventStatus.Active))
            return EventErrors.EventVisibility.ActiveEventCannotBePrivate;
        
        if (visibility.Equals(EventVisibility.Private))
            return new None();
        
        visibility = EventVisibility.Private;
        
        if (status.Equals(EventStatus.Ready))
            status = EventStatus.Draft;
        
        return new None();
    }

    public Result<None> SetReadyStatus(ICurrentTime currentTime)
    {
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventReadyStatus.CancelledEventCannotBeReadied;
        
        if (!status.Equals(EventStatus.Draft))
            return EventErrors.EventReadyStatus.NotInDraftStatus;
        
        if (title.Value.Equals("Working Title"))
            return EventErrors.EventReadyStatus.TitleIsDefault;
        
        if (eventTime == null)
            return EventErrors.EventReadyStatus.TimesNotSet;
        
        if (eventTime.StartTime < currentTime.GetCurrentTime())
            return EventErrors.EventReadyStatus.StartTimeInPast;

        if (maxGuests.Value < 5 || maxGuests.Value > 50)
            return EventErrors.EventReadyStatus.MaxGuestsInvalid;
        
        status = EventStatus.Ready;
        return new None();
    }
    
    public Result<None> SetActiveStatus(ICurrentTime currentTime)
    {
        if (status.Equals(EventStatus.Active))
            return new None();
        
        if (status.Equals(EventStatus.Cancelled))
            return EventErrors.EventActiveStatus.CancelledEventCannotBeActivated;

        if (status.Equals(EventStatus.Draft))
        {
            var readyResult = SetReadyStatus(currentTime);
            
            if (readyResult.IsFailure)
                return readyResult;
        }
        
        status = EventStatus.Active;
        
        return new None();
    }
    
    public Result<None> SetCancelledStatus()
    {
        status = EventStatus.Cancelled;
        return new None();
    }

    public Result<None> AssignGuestToGuestList(Guest guest, ICurrentTime currentTime)
    {
        if (!status.Equals(EventStatus.Active))
            return GuestListErrors.Participation.EventNotActive;

        if (!visibility.Equals(EventVisibility.Public))
            return GuestListErrors.Participation.EventIsPrivate;
        
        if (eventTime == null || eventTime.StartTime <= currentTime.GetCurrentTime())
            return GuestListErrors.Participation.EventAlreadyStarted;

        if (guestList.numberOfGuests >= maxGuests.Value)
            return GuestListErrors.Participation.NoMoreRoom;

        return guestList.AssignToGuestList(guest.Id);
    }

    public Result<None> RemoveGuestFromGuestList(Guest guest, ICurrentTime currentTime)
    {
        if (eventTime != null && eventTime.StartTime <= currentTime.GetCurrentTime())
            return GuestListErrors.Participation.CannotCancelPastEvent;
        
        return guestList.RemoveFromGuestList(guest.Id);
    }
}