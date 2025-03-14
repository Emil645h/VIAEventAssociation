using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;

public sealed record LocationId
{
    
    public Guid Value { get; }
    
    private LocationId(Guid input) => Value = input;

    public static Result<LocationId> Create(Guid value) =>
    value == Guid.Empty ? LocationsErrors.LocationID.IsEmpty : new LocationId(value);
    
}