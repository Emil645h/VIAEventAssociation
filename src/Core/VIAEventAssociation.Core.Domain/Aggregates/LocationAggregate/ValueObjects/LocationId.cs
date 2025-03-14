using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Locations.ValueObjects;

public record LocationId
{
    
    public Guid Value { get; }
    
    private LocationId(Guid input) => Value = input;

    public static Result<LocationId> Create(Guid locationId) => locationId == Guid.Empty ? LocationErrors.LocationId.IsEmpty : new LocationId(locationId);
    
}