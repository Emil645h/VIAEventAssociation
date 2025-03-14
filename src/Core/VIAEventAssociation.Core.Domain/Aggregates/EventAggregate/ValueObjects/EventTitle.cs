using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public record EventTitle 
{
    internal string Value { get; }
    
    internal EventTitle(string input) =>
        Value = input;

    public static Result<EventTitle> Create(string eventTitle)
    {
        return new EventTitle(eventTitle);
    }
    
}