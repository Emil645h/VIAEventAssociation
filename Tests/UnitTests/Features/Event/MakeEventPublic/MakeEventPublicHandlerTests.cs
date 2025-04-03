using System.Reflection;
using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.MakeEventPublic;

public class MakeEventPublicHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly MakeEventPublicHandler handler;

    public MakeEventPublicHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Create the handler with fakes
        handler = new MakeEventPublicHandler(eventRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithEventInDraftState_MakesEventPublic()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // The event is private by default, so no need to modify visibility
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPublicCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        // Check visibility field using reflection since it's internal
        var visibilityField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("visibility", BindingFlags.Instance | BindingFlags.NonPublic);
        var visibility = visibilityField.GetValue(getEventResult.Value);
        Assert.Equal(EventVisibility.Public, visibility);
    }
    
    [Fact]
    public async Task HandleAsync_WithEventInReadyState_MakesEventPublic()
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
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = MakeEventPublicCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        // Verify event was updated and status unchanged
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Ready, status);
        
        var visibilityField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("visibility", BindingFlags.Instance | BindingFlags.NonPublic);
        var visibility = visibilityField.GetValue(getEventResult.Value);
        Assert.Equal(EventVisibility.Public, visibility);
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
        var commandResult = MakeEventPublicCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.CancelledEventCannotBeModified);
    }
}