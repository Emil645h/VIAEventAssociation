using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

public sealed record GuestId
{
    public Guid Value { get; }

    private GuestId(Guid value) => Value = value;

    public static Result<GuestId> Create(Guid value) =>
        value == Guid.Empty ? GuestErrors.GuestId.IsEmpty : new GuestId(value);
    
    public static Result<GuestId> FromString(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return GuestErrors.GuestId.IsEmpty;

        if (!Guid.TryParse(id, out var guidResult))
            return GuestErrors.GuestId.IsEmpty;
        
        return new GuestId(guidResult);
    }
}