using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public record EventMaxGuests
{
    internal int Value { get; }
    
    internal EventMaxGuests(int input) =>
        Value = input;
    
    public static Result<EventMaxGuests> Create(int input)
    {
        return new EventMaxGuests(input);
    }
}