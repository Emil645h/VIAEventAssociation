using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Locations.ValueObjects;

public record LocationId
{
    
    internal Guid Value { get; }
    
    internal LocationId(Guid input) => Value = input;

    public static Result<LocationId> Create(Guid locationId)
    {
        return new Success<LocationId>(new LocationId(locationId));
    }
    
}