using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventDescription;

public class UpdateEventDescriptionCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccessResult()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var validDescription = "This is a valid description";

        // Act
        var result = UpdateEventDescriptionCommand.Create(validId, validDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validId, result.Value.Id.Value.ToString());
        Assert.Equal(validDescription, result.Value.Description.Value);
    }

    [Fact]
    public void Create_WithInvalidGuidFormat_ReturnsError()
    {
        // Arrange
        var invalidId = "not-a-guid";
        var validDescription = "This is a valid description";

        // Act
        var result = UpdateEventDescriptionCommand.Create(invalidId, validDescription);
        var resultFailure = Assert.IsType<Failure<UpdateEventDescriptionCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var tooLongDescription = new string('x', 251); // More than 250 characters

        // Act
        var result = UpdateEventDescriptionCommand.Create(validId, tooLongDescription);
        var resultFailure = Assert.IsType<Failure<UpdateEventDescriptionCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventDescription.InvalidCharacterLength);
    }
}