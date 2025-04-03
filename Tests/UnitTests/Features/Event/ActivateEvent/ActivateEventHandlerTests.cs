using System.Reflection;
using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.ActivateEvent;

public class ActivateEventHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly StubCurrentTime currentTimeStub;
    private readonly ActivateEventHandler handler;

    public ActivateEventHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        currentTimeStub = new StubCurrentTime(new DateTime(2023, 8, 20, 12, 0, 0));
        
        // Create the handler with fakes
        handler = new ActivateEventHandler(eventRepository, unitOfWork, currentTimeStub);
    }

    [Fact]
    public async Task HandleAsync_WithValidEventInReadyState_MakesEventActive()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set a proper title (not default "Working Title")
        var title = EventTitle.Create("My Event").Value;
        existingEvent.UpdateTitle(title);
        
        // Set event time in the future
        var startTime = currentTimeStub.TheTime.AddDays(5);
        var endTime = startTime.AddHours(3);
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        existingEvent.UpdateTime(eventTimeResult.Value);
        
        // Set status to Ready
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Ready);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ActivateEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated to Active status
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Active, status);
    }
    
    [Fact]
    public async Task HandleAsync_WithValidEventInDraftState_MakesEventReadyThenActive()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set a proper title (not default "Working Title")
        var title = EventTitle.Create("My Event").Value;
        existingEvent.UpdateTitle(title);
        
        // Set event time in the future
        var startTime = currentTimeStub.TheTime.AddDays(5);
        var endTime = startTime.AddHours(3);
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        existingEvent.UpdateTime(eventTimeResult.Value);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ActivateEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated to Active status
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Active, status);
    }
    
    [Fact]
    public async Task HandleAsync_WithEventAlreadyActive_LeavesEventActive()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set a proper title (not default "Working Title")
        var title = EventTitle.Create("My Event").Value;
        existingEvent.UpdateTitle(title);
        
        // Set event time in the future
        var startTime = currentTimeStub.TheTime.AddDays(5);
        var endTime = startTime.AddHours(3);
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        existingEvent.UpdateTime(eventTimeResult.Value);
        
        // Set status to Active
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Active);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ActivateEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event remains Active status
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Active, status);
    }
    
    [Fact]
    public async Task HandleAsync_WithDraftEventNotReadyable_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event with default title (not ready-able)
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ActivateEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        // Should show failure trying to ready the event
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TitleIsDefault);
    }
}