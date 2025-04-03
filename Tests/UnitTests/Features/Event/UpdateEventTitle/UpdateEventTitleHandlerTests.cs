using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventTitle;

public class UpdateEventTitleHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly UpdateEventTitleHandler handler;

    public UpdateEventTitleHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Create the handler with fakes
        handler = new UpdateEventTitleHandler(eventRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithDraftEvent_UpdatesTitleAndReturnSuccess()
    {
        // Arrange
        // Create an event with factory method
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create a valid title and command
        var newTitleValue = "My Updated Event";
        var newTitleResult = EventTitle.Create(newTitleValue);
        Assert.True(newTitleResult.IsSuccess);
        
        var commandResult = UpdateEventTitleCommand.Create(eventId.Value.ToString(), newTitleValue);
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        Assert.Equal(newTitleValue, getEventResult.Value.title.Value);
    }
    
    [Fact]
    public async Task HandleAsync_WithActiveEvent_ReturnsFailure()
    {
        // Arrange
        // Create an event
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Manually set to active status (since we might not have all prerequisites for SetActiveStatus)
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Active);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create command with new title
        var newTitleValue = "My Updated Event";
        var commandResult = UpdateEventTitleCommand.Create(eventId.Value.ToString(), newTitleValue);
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidEventStatus);
        
        // Verify event was not updated
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        Assert.NotEqual(newTitleValue, getEventResult.Value.title.Value);
    }
}