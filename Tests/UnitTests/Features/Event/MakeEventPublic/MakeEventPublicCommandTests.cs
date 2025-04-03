using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.MakeEventPublic;

public class MakeEventPublicCommandTests
{
    [Fact]
    public void Create_WithValidId_ReturnsSuccessResult()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();

        // Act
        var result = MakeEventPublicCommand.Create(validId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validId, result.Value.Id.Value.ToString());
    }

    [Fact]
    public void Create_WithInvalidGuidFormat_ReturnsError()
    {
        // Arrange
        var invalidId = "not-a-guid";

        // Act
        var result = MakeEventPublicCommand.Create(invalidId);
        var resultFailure = Assert.IsType<Failure<MakeEventPublicCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }
}