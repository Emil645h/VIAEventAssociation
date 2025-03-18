using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventDescription
{
    internal string Value { get; }
    
    internal EventDescription(string input) =>
        Value = input;

    public static Result<EventDescription> Create(string eventDescription)
        => Validate(eventDescription);

    private static Result<EventDescription> Validate(string eventDescription) =>
        ResultExtensions.AssertAll(
            () => MustNotBeLessThan0OrMoreThan250Characters(eventDescription)
        ).WithPayloadIfSuccess(() => new EventDescription(eventDescription));

    public static Result<None> MustNotBeLessThan0OrMoreThan250Characters(string eventDescription)
    {
        if (eventDescription.Length > 250)
        {
            return EventErrors.EventDescription.InvalidCharacterLength;
        }

        return new None();
    }
    
}