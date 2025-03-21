using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public record EventTitle 
{
    internal string Value { get; }
    
    private EventTitle(string input) =>
        Value = input;

    public static Result<EventTitle> Create(string eventTitle) 
        => eventTitle == null ? EventErrors.EventTitle.TitleIsNull : Validate(eventTitle);
    
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

    private static Result<EventTitle> Validate(string eventTitle) =>
        ResultExtensions.AssertAll(
            () => IsEmpty(eventTitle),
            () => CharacterLimit(eventTitle))
            .WithPayloadIfSuccess(() => new EventTitle(eventTitle));
}