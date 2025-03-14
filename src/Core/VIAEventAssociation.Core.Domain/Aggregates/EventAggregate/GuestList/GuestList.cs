using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;

public class GuestList : Entity<GuestListId>
{
    internal int numberOfGuests => guests.Count;
    private readonly HashSet<GuestId> guests;
    
    private GuestList(GuestListId id) : base(id)
        => this.guests = new();

    public static Result<GuestList> Create(GuestListId id)
        => new GuestList(id);

    public Result<None> AssignToGuestList(GuestId guestId)
    {
        if (guestId == null)
            return GuestListErrors.AssignToGuestList.GuestIsEmpty;
        
        if (!guests.Add(guestId))
            return GuestListErrors.AssignToGuestList.GuestAlreadyAssigned;

        return new None();
    }
}