using System.Reflection;
using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.ReadyEvent;

public class ReadyEventHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly StubCurrentTime currentTimeStub;
    private readonly ReadyEventHandler handler;

    public ReadyEventHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        currentTimeStub = new StubCurrentTime(new DateTime(2023, 8, 20, 12, 0, 0));
        
        // Create the handler with fakes
        handler = new ReadyEventHandler(eventRepository, unitOfWork, currentTimeStub);
    }

    [Fact]
    public async Task HandleAsync_WithValidEventInDraftState_MakesEventReady()
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
        var commandResult = ReadyEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated to Ready status
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic);
        var status = statusField.GetValue(getEventResult.Value);
        Assert.Equal(EventStatus.Ready, status);
    }
    
    [Fact]
    public async Task HandleAsync_WithDefaultTitle_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event (which has "Working Title" by default)
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set event time in the future
        var startTime = currentTimeStub.TheTime.AddDays(5);
        var endTime = startTime.AddHours(3);
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        existingEvent.UpdateTime(eventTimeResult.Value);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ReadyEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TitleIsDefault);
    }
    
    [Fact]
    public async Task HandleAsync_WithoutTime_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set a proper title (not default)
        var title = EventTitle.Create("My Event").Value;
        existingEvent.UpdateTitle(title);
        
        // Don't set event time
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ReadyEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TimesNotSet);
    }
    
    [Fact]
    public async Task HandleAsync_WithPastStartTime_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set a proper title (not default)
        var title = EventTitle.Create("My Event").Value;
        existingEvent.UpdateTitle(title);
        
        // Set event time in the past
        var startTime = currentTimeStub.TheTime.AddDays(-1); // 1 day in the past
        var endTime = startTime.AddHours(3);
        
        // Need to temporarily adjust current time to create past event time
        var originalTime = currentTimeStub.TheTime;
        currentTimeStub.TheTime = startTime.AddDays(-2); // Set current time to 2 days before start time
        
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        existingEvent.UpdateTime(eventTimeResult.Value);
        
        // Reset current time for the test
        currentTimeStub.TheTime = originalTime;
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ReadyEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.StartTimeInPast);
    }
    
    [Fact]
    public async Task HandleAsync_WithEventNotInDraftState_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Set status to something other than Draft (e.g., Cancelled)
        typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(existingEvent, EventStatus.Cancelled);
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create and validate command
        var commandResult = ReadyEventCommand.Create(eventId.Value.ToString());
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.CancelledEventCannotBeReadied);
    }
}