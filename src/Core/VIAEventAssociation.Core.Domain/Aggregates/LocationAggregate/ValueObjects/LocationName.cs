using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;

public record LocationName
{
    internal string Value { get; }
    
    private LocationName(string input) =>
        Value = input;

    public static Result<LocationName> Create(string input) 
        => string.IsNullOrWhiteSpace(input) ? LocationsErrors.LocationName.IsEmpty : new LocationName(input);
}