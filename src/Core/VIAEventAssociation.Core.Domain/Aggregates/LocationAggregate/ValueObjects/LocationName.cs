using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;

public class LocationName
{
    internal string Value { get; }
    
    internal LocationName(string input) =>
        Value = input;

    public static Result<LocationName> Create(string input) 
        => string.IsNullOrWhiteSpace(input) ? LocationsErrors.LocationName.IsEmpty : new LocationName(input);
}