using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventMaxGuests;

public class UpdateEventMaxGuestsCommandTests
{
    [Fact]
    public void Create_WithValidIdAndMaxGuests_ReturnsSuccessResult()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var validMaxGuests = 25;

        // Act
        var result = UpdateEventMaxGuestsCommand.Create(validId, validMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validId, result.Value.Id.Value.ToString());
        Assert.Equal(validMaxGuests, result.Value.MaxGuests.Value);
    }

    [Fact]
    public void Create_WithInvalidGuidFormat_ReturnsError()
    {
        // Arrange
        var invalidId = "not-a-guid";
        var validMaxGuests = 25;

        // Act
        var result = UpdateEventMaxGuestsCommand.Create(invalidId, validMaxGuests);
        var resultFailure = Assert.IsType<Failure<UpdateEventMaxGuestsCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }

    [Fact]
    public void Create_WithMaxGuestsTooSmall_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var tooSmallMaxGuests = 4; // Less than minimum (5)

        // Act
        var result = UpdateEventMaxGuestsCommand.Create(validId, tooSmallMaxGuests);
        var resultFailure = Assert.IsType<Failure<UpdateEventMaxGuestsCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.TooSmall);
    }

    [Fact]
    public void Create_WithMaxGuestsTooLarge_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var tooLargeMaxGuests = 51; // More than maximum (50)

        // Act
        var result = UpdateEventMaxGuestsCommand.Create(validId, tooLargeMaxGuests);
        var resultFailure = Assert.IsType<Failure<UpdateEventMaxGuestsCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.TooLarge);
    }
}