using VIAEventAssociation.Core.Domain.Aggregates.Events.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.GuestList;

public class GuestList : Entity<GuestListId>
{
    internal NumberOfGuests numberOfGuests;
    
    private GuestList(GuestListId id, NumberOfGuests numberOfGuests) : base(id)
        => this.numberOfGuests = numberOfGuests;
    
    public static Result<GuestList> Create(GuestListId id, NumberOfGuests numberOfGuests)
    => new GuestList(id, numberOfGuests);
}