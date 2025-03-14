using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventTitle 
{
    internal string Value { get; }
    
    internal EventTitle(string input) =>
        Value = input;

    public static Result<EventTitle> Create(string eventTitle) =>
        Validate(eventTitle);
    
    private static Result<None> CharacterLimit(string eventTitle)
    {
        if (eventTitle.Length < 3 || eventTitle.Length > 75)
            return EventErrors.EventTitle.InvalidTitleCharacterLimit;
        return new None();
    }
    
    private static Result<None> IsEmpty(string eventTitle)
    {
        if (string.IsNullOrWhiteSpace(eventTitle))
            return EventErrors.EventTitle.TitleIsEmpty;
        return new None();
    }
    
    private static Result<None> IsNull(string eventTitle)
    {
        if (eventTitle == null)
            return EventErrors.EventTitle.TitleIsNull;
        return new None();
    }

    private static Result<EventTitle> Validate(string eventTitle) =>
        ResultExtensions.AssertAll(() => CharacterLimit(eventTitle),
            () => IsEmpty(eventTitle), () => IsNull(eventTitle)).WithPayloadIfSuccess(() => new EventTitle(eventTitle));
}