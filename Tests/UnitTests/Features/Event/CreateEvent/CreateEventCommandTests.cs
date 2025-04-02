using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventCommandTests
{
    [Fact]
    public void Create_WithValidId_ReturnsCommand()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        
        // Act
        var result = CreateEventCommand.Create(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Id);
        Assert.Equal(Guid.Parse(id), result.Value.Id.Value);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyId_ReturnsError(string id)
    {
        // Act
        var result = CreateEventCommand.Create(id);
        var resultFailure = Assert.IsType<Failure<CreateEventCommand>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        // Check that we get the expected error from the EventId domain class
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }
    
    [Fact]
    public void Create_WithInvalidGuidFormat_ReturnsError()
    {
        // Arrange
        var invalidId = "not-a-guid";
        
        // Act
        var result = CreateEventCommand.Create(invalidId);
        var resultFailure = Assert.IsType<Failure<CreateEventCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        // Check that we get the expected error from the EventId domain class
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventId.IsEmpty);
    }
}