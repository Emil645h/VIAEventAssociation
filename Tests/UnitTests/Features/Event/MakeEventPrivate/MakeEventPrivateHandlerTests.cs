using System.Reflection;
using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.MakeEventPrivate;

public class MakeEventPrivateHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly MakeEventPrivateHandler handler;

    public MakeEventPrivateHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Create the handler with fakes
        handler = new MakeEventPrivateHandler(eventRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithAlreadyPrivateEvent_LeavesEventUnchanged()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event (which is private by default)
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set to ready status using reflection to verify status doesn't change
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Ready);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPrivateCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event status and visibility remained the same
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Ready, status); // Status should remain Ready
        
        var visibilityField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("visibility", BindingFlags.Instance | BindingFlags.NonPublic);
        var visibility = visibilityField.GetValue(getEventResult.Value);
        Assert.Equal(EventVisibility.Private, visibility); // Should still be Private
    }
    
    [Fact]
    public async Task HandleAsync_WithPublicEventInReadyState_MakesEventPrivateAndChangeStatusToDraft()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set to ready status using reflection
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Ready);
            
        // Make the event public first
        existingEvent.MakePublic();
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPrivateCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        // Verify event was updated to private and status changed to draft
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Draft, status); // Status should be Draft
        
        var visibilityField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("visibility", BindingFlags.Instance | BindingFlags.NonPublic);
        var visibility = visibilityField.GetValue(getEventResult.Value);
        Assert.Equal(EventVisibility.Private, visibility); // Should be Private
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
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Active);
            
        // Make event public
        existingEvent.MakePublic();
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPrivateCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.ActiveEventCannotBePrivate);
    }
    
    [Fact]
    public async Task HandleAsync_WithEventInCancelledState_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set to cancelled status using reflection
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Cancelled);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPrivateCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.CancelledEventCannotBeModified);
    }
}