using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;

public sealed record GuestListId
{
    public Guid Value { get; }
    
    private GuestListId(Guid value) => Value = value;
    
    public static Result<GuestListId> Create(Guid value) =>
    value == Guid.Empty ? GuestListErrors.GuestListId.IsEmpty : new GuestListId(value);
}