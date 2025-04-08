using Moq;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.CommandDispatching.Dispatcher;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.CustomExceptions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Application;

public class CommandDispatcherTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly CommandDispatcher _dispatcher;
    
    public CommandDispatcherTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _dispatcher = new CommandDispatcher(_serviceProviderMock.Object);
    }
    
    [Fact]
    public async Task DispatchAsync_WithCreateEventCommand_ShouldCallHandleAsync()
    {
        // Arrange
        var successResult = new Success<None>(new None());
        var createEventHandlerMock = new Mock<ICommandHandler<CreateEventCommand>>();
        createEventHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CreateEventCommand>()))
            .ReturnsAsync(successResult);
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<CreateEventCommand>)))
            .Returns(createEventHandlerMock.Object);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var createEventCommand = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act
        var result = await _dispatcher.DispatchAsync(createEventCommand);
        
        // Assert
        createEventHandlerMock.Verify(h => h.HandleAsync(createEventCommand), Times.Once);
        Assert.IsType<Success<None>>(result);
    }
    
    [Fact]
    public async Task DispatchAsync_WithUpdateEventTitleCommand_ShouldCallHandleAsync()
    {
        // Arrange
        var successResult = new Success<None>(new None());
        var updateEventTitleHandlerMock = new Mock<ICommandHandler<UpdateEventTitleCommand>>();
        updateEventTitleHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<UpdateEventTitleCommand>()))
            .ReturnsAsync(successResult);
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<UpdateEventTitleCommand>)))
            .Returns(updateEventTitleHandlerMock.Object);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var eventTitle = EventTitle.Create("New Title").Value;
        var updateEventTitleCommand = UpdateEventTitleCommand.Create(eventId.Value.ToString(), eventTitle.Value).Value;
        
        // Act
        var result = await _dispatcher.DispatchAsync(updateEventTitleCommand);
        
        // Assert
        updateEventTitleHandlerMock.Verify(h => h.HandleAsync(updateEventTitleCommand), Times.Once);
        Assert.IsType<Success<None>>(result);
    }
    
    [Fact]
    public async Task DispatchAsync_WithNoHandler_ShouldThrowServiceNotFoundException()
    {
        // Arrange
        _serviceProviderMock
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(null);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var createEventCommand = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act & Assert
        await Assert.ThrowsAsync<ServiceNotFoundException>(() => _dispatcher.DispatchAsync(createEventCommand));
    }
    
    [Fact]
    public async Task DispatchAsync_WhenHandlerReturnsFailure_ShouldReturnFailure()
    {
        // Arrange
        var errors = new List<ResultError> { new ResultError("TEST_ERROR", "Test failure") };
        var failureResult = new Failure<None>(errors);
        
        var handlerMock = new Mock<ICommandHandler<CreateEventCommand>>();
        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CreateEventCommand>()))
            .ReturnsAsync(failureResult);
        
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<CreateEventCommand>)))
            .Returns(handlerMock.Object);
        
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var command = CreateEventCommand.Create(eventId.Value.ToString()).Value;
        
        // Act
        var result = await _dispatcher.DispatchAsync(command);
        
        // Assert
        handlerMock.Verify(h => h.HandleAsync(command), Times.Once);
        Assert.IsType<Failure<None>>(result);
        var failureResponse = result as Failure<None>;
        Assert.NotNull(failureResponse);
        Assert.Contains(failureResponse.Errors, e => e.Code == "TEST_ERROR" && e.Message == "Test failure");
    }
}