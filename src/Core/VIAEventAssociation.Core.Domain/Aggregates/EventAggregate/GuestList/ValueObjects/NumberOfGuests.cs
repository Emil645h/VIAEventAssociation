using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.GuestList.ValueObjects;

public record NumberOfGuests
{
    internal int Value { get; }
    
    internal NumberOfGuests(int value) => Value = value;
    
    public static Result<NumberOfGuests> Create(int numberOfGuests) => Validate(numberOfGuests);
    
    
    private static Result<NumberOfGuests> Validate(int numberOfGuests) =>
    ResultExtensions.AssertAll(() => MustBeInRange(numberOfGuests)
        ).WithPayloadIfSuccess(() => new NumberOfGuests(numberOfGuests));

    private static Result<None> MustBeInRange(int numberOfGuests)
    {
        if (numberOfGuests <= 0 || numberOfGuests > 100)
            return GuestListErrors.NumberOfGuests.IsInvalidRange;
        return new Success<None>(new None());
    }
    
}