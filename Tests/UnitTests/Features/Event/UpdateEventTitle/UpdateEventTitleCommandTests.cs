using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventTitle;

public class UpdateEventTitleCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccessResult()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var validTitle = "Valid Event Title";

        // Act
        var result = UpdateEventTitleCommand.Create(validId, validTitle);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validId, result.Value.Id.Value.ToString());
        Assert.Equal(validTitle, result.Value.Title.Value);
    }

    [Fact]
    public void Create_WithInvalidGuidFormat_ReturnsError()
    {
        // Arrange
        var invalidId = "not-a-guid";
        var validTitle = "Valid Event Title";

        // Act
        var result = UpdateEventTitleCommand.Create(invalidId, validTitle);
        var resultFailure = Assert.IsType<Failure<UpdateEventTitleCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var emptyTitle = "";

        // Act
        var result = UpdateEventTitleCommand.Create(validId, emptyTitle);
        var resultFailure = Assert.IsType<Failure<UpdateEventTitleCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.TitleIsEmpty);
    }

    [Fact]
    public void Create_WithNullTitle_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        string? nullTitle = null;

        // Act
        var result = UpdateEventTitleCommand.Create(validId, nullTitle);
        var resultFailure = Assert.IsType<Failure<UpdateEventTitleCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.TitleIsNull);
    }

    [Fact]
    public void Create_WithTitleTooShort_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var tooShortTitle = "ab"; // Less than 3 characters

        // Act
        var result = UpdateEventTitleCommand.Create(validId, tooShortTitle);
        var resultFailure = Assert.IsType<Failure<UpdateEventTitleCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidTitleCharacterLimit);
    }

    [Fact]
    public void Create_WithTitleTooLong_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var tooLongTitle = new string('x', 76); // More than 75 characters

        // Act
        var result = UpdateEventTitleCommand.Create(validId, tooLongTitle);
        var resultFailure = Assert.IsType<Failure<UpdateEventTitleCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidTitleCharacterLimit);
    }
}