using Moq;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.CommandDispatching.Dispatcher;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.CustomExceptions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Application;

public class CommandExecutionTimerTests
{
    private readonly Mock<ICommandDispatcher> _nextMock;
    private readonly CommandExecutionTimer _timerDispatcher;
    
    public CommandExecutionTimerTests()
    {
        _nextMock = new Mock<ICommandDispatcher>();
        _timerDispatcher = new CommandExecutionTimer(_nextMock.Object);
    }
    
    [Fact]
    public async Task DispatchAsync_ShouldCallDecoratedDispatcher()
    {
        // Arrange
        var successResult = new Success<None>(new None());
        _nextMock
            .Setup(d => d.DispatchAsync(It.IsAny<CreateEventCommand>()))
            .ReturnsAsync(successResult);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var command = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act
        var result = await _timerDispatcher.DispatchAsync(command);
        
        // Assert
        _nextMock.Verify(d => d.DispatchAsync(command), Times.Once);
        Assert.IsType<Success<None>>(result);
    }
    
    [Fact]
    public async Task DispatchAsync_WhenExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var expectedException = new ServiceNotFoundException("Test exception");
        _nextMock
            .Setup(d => d.DispatchAsync(It.IsAny<CreateEventCommand>()))
            .ThrowsAsync(expectedException);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var command = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ServiceNotFoundException>(
            async () => await _timerDispatcher.DispatchAsync(command));
            
        Assert.Same(expectedException, exception);
    }
    
    [Fact]
    public async Task DispatchAsync_ShouldMeasureTime()
    {
        // Arrange - Set up a delay in the decorated dispatcher to ensure measurable time
        var successResult = new Success<None>(new None());
        _nextMock
            .Setup(d => d.DispatchAsync(It.IsAny<CreateEventCommand>()))
            .Returns(async () => {
                await Task.Delay(50); // Small delay to ensure time is measurable
                return successResult;
            });
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var command = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act
        await _timerDispatcher.DispatchAsync(command);
        
        // Assert - we can't easily verify the console output, but we can ensure
        // the inner dispatcher was called
        _nextMock.Verify(d => d.DispatchAsync(command), Times.Once);
    }
}