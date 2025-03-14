namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public record EventMaxGuests
{
    internal int Value { get; }
    
    internal EventMaxGuests(int input) =>
        Value = input;
    
    public static EventMaxGuests Create(int input)
    {
        return new EventMaxGuests(input);
    }
}