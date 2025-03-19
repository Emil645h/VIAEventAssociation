using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventMaxGuests
{
    internal int Value { get; }
    
    internal EventMaxGuests(int input) =>
        Value = input;

    public static Result<EventMaxGuests> Create(int input)
        => Validate(input);
    
    private static Result<EventMaxGuests> Validate(int input) 
        => ResultExtensions.AssertAll(
            () => ValidateMinMaxGuests(input)
            ).WithPayloadIfSuccess(() => new EventMaxGuests(input));

    private static Result<None> ValidateMinMaxGuests(int input)
    {
        if (input < 5)
            return EventErrors.EventMaxGuests.TooSmall;

        if (input > 50)
        {
            return EventErrors.EventMaxGuests.TooLarge;
        }

        return new None();
    }
}