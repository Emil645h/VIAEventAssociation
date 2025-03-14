using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Locations.ValueObjects;

public record LocationName
{
    internal string Value { get; }
    
    internal LocationName(string input) =>
        Value = input;

    public static Result<LocationName> Create(string input)
    {
        return new LocationName(input);
    }
    
}