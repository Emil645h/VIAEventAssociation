using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventDescription;

public class UpdateEventDescriptionHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly UpdateEventDescriptionHandler handler;

    public UpdateEventDescriptionHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Create the handler with fakes
        handler = new UpdateEventDescriptionHandler(eventRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithEventInDraftState_UpdatesDescription()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create a valid description and command
        var newDescriptionValue = "New event description";
        var commandResult = UpdateEventDescriptionCommand.Create(eventId.Value.ToString(), newDescriptionValue);
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        Assert.Equal(newDescriptionValue, getEventResult.Value.description.Value);
    }
    
    [Fact]
    public async Task HandleAsync_WithEventInActiveState_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set to active status using reflection
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Active);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create command with new description
        var newDescriptionValue = "New event description";
        var commandResult = UpdateEventDescriptionCommand.Create(eventId.Value.ToString(), newDescriptionValue);
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventDescription.InvalidEventStatus);
    }
}