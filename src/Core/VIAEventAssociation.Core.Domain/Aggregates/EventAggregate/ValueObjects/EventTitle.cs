using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventTitle 
{
    internal string Value { get; }
    
    internal EventTitle(string input) =>
        Value = input;

    public static Result<EventTitle> Create(string eventTitle)
    {
        return new EventTitle(eventTitle);
    }
    
}