using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;

public class Event : AggregateRoot<EventId>
{
    internal EventTitle title;
    internal EventDescription description;
    internal DateTime? startDate;
    internal DateTime? endDate;
    internal EventVisibility visibility;
    internal EventStatus status;
    internal EventMaxGuests maxGuests;
    internal GuestList.GuestList guestList;
    internal LocationId? locationId;
    
    private Event(EventId id, EventTitle title, EventDescription description, DateTime startDate, DateTime endDate, EventVisibility visibility, EventStatus status, EventMaxGuests maxGuests, GuestList.GuestList guestList, LocationId locationId)
        : base(id)
    {
        (this.title, this.description, this.startDate, this.endDate, this.visibility, this.status, this.maxGuests, this.guestList, this.locationId)
            = (title, description, startDate, endDate, visibility, status, maxGuests, guestList, locationId);
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
        
        return new Event(id, title, description, default, default, visibility, status, maxGuests, guestList, null);
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

    public Result<None> MakePublic()
    {
        visibility = EventVisibility.Public;
        return new None();
    }
    
    public Result<None> MakePrivate()
    {
        visibility = EventVisibility.Private;
        return new None();
    }

    public Result<None> SetReadyStatus()
    {
        status = EventStatus.Ready;
        return new None();
    }
    
    public Result<None> SetActiveStatus()
    {
        status = EventStatus.Active;
        return new None();
    }
    
    public Result<None> SetCancelledStatus()
    {
        status = EventStatus.Cancelled;
        return new None();
    }
}