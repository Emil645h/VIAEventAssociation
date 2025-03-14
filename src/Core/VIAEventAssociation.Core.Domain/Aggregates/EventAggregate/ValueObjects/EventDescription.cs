namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public record EventDescription
{
    internal string Value { get; }
    
    internal EventDescription(string input) =>
        Value = input;
    
    public static EventDescription Create(string eventDescription)
    {
        return new EventDescription(eventDescription);
    }
}