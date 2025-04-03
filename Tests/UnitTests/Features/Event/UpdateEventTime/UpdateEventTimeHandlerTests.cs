using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventTime;

public class UpdateEventTimeHandlerTests
{
    private readonly FakeEventRepository eventRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly UpdateEventTimeHandler handler;
    private readonly StubCurrentTime currentTimeStub;

    public UpdateEventTimeHandlerTests()
    {
        // Setup the fakes
        eventRepository = new FakeEventRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Setup current time stub
        currentTimeStub = new StubCurrentTime(new DateTime(2023, 8, 20, 12, 0, 0));
        
        // Create the handler with fakes
        handler = new UpdateEventTimeHandler(eventRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithEventInDraftState_UpdatesEventTime()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        
        // Create an event
        var eventResult = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId);
        Assert.True(eventResult.IsSuccess);
        var existingEvent = eventResult.Value;
        
        // Add to repository
        await eventRepository.AddAsync(existingEvent);
        
        // Create a valid event time
        var startTime = currentTimeStub.TheTime.AddDays(5).Date.AddHours(13); // 1 PM in 5 days
        var endTime = currentTimeStub.TheTime.AddDays(5).Date.AddHours(16);   // 4 PM in 5 days
        
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTimeStub);
        Assert.True(eventTimeResult.IsSuccess);
        
        // Create and validate command
        var commandResult = UpdateEventTimeCommand.Create(
            eventId.Value.ToString(), 
            startTime, 
            endTime, 
            currentTimeStub);
        
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify event was updated
        var getEventResult = await eventRepository.GetByIdAsync(eventId);
        Assert.True(getEventResult.IsSuccess);
        Assert.NotNull(getEventResult.Value.eventTime);
        Assert.Equal(startTime, getEventResult.Value.eventTime.StartTime);
        Assert.Equal(endTime, getEventResult.Value.eventTime.EndTime);
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
        
        // Create valid times
        var startTime = currentTimeStub.TheTime.AddDays(5).Date.AddHours(13);
        var endTime = currentTimeStub.TheTime.AddDays(5).Date.AddHours(16);
        
        // Create and validate command
        var commandResult = UpdateEventTimeCommand.Create(
            eventId.Value.ToString(), 
            startTime, 
            endTime, 
            currentTimeStub);
        
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.ActiveEventCannotBeModified);
    }
}