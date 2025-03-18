using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.GuestListTests;

public class NumberOfGuestsTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Create_WithValidNumberOfGuests_ReturnsNumberOfGuests(int guestNumber)
    {
        var result = NumberOfGuests.Create(guestNumber);
        
        Assert.True(result.IsSuccess);
        
        var success = Assert.IsType<Success<NumberOfGuests>>(result);
        var numberOfGuests = success.Value;
        Assert.Equal(guestNumber, numberOfGuests.Value);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(-1)]
    public void Create_WithInvalidNumberOfGuests_ReturnsFailure(int guestNumber)
    {
        var result = NumberOfGuests.Create(guestNumber);
        
        Assert.True(result.IsFailure);
        var failure = Assert.IsType<Failure<NumberOfGuests>>(result);
        Assert.Contains(failure.Errors, e => e == GuestListErrors.NumberOfGuests.IsInvalidRange);
    }
}