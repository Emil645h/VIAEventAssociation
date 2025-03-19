using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventStatusTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2025, 1, 1, 12, 0, 0));

    private Event CreateValidDraftEvent()
    {
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set valid title
        var title = EventTitle.Create("Valid Title").Value;
        _event.UpdateTitle(title);
        
        // Set valid time (in the future)
        var startTime = new DateTime(2050, 1, 2, 12, 0, 0);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        
        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        return _event;
    }

    #region Ready Status

    // S1
    [Fact]
    public void SetReadyStatus_ValidEvent_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        
        // Act
        var result = _event.SetReadyStatus(_defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Ready, _event.status);
    }
    
    // F2
    [Fact]
    public void SetReadyStatus_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        _event.SetCancelledStatus();
        
        // Act
        var result = _event.SetReadyStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.CancelledEventCannotBeReadied);
    }
    
    [Fact]
    public void SetReadyStatus_ActiveEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        _event.SetReadyStatus(_defaultTime); // First make it ready
        _event.SetActiveStatus(_defaultTime); // Then make it active
        
        // Act
        var result = _event.SetReadyStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.NotInDraftStatus);
    }
    
    // F1
    [Fact]
    public void SetReadyStatus_DefaultTitle_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set everything except title
        var startTime = new DateTime(2050, 1, 2, 12, 0,0);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Act
        var result = _event.SetReadyStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TitleIsDefault);
    }
    
    // F1
    [Fact]
    public void SetReadyStatus_TimesNotSet_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set everything except times
        var title = EventTitle.Create("Valid Title").Value;
        _event.UpdateTitle(title);
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Act
        var result = _event.SetReadyStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TimesNotSet);
    }
    
    // F3
    [Fact]
    public void SetReadyStatus_StartTimeInPast_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set valid title
        var title = EventTitle.Create("Valid Title").Value;
        _event.UpdateTitle(title);
        
        // Set time in the "past" (before our stub time)
        var stubTime = new StubCurrentTime(new DateTime(2050, 1, 5, 12, 0, 0)); // Jan 5, 2050
        var startTime = new DateTime(2050, 1, 3, 12, 0, 0); // Jan 3, 2050 (before stubTime)
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        
        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Act
        var result = _event.SetReadyStatus(stubTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.StartTimeInPast);
    }

    #endregion

    #region Active Status

    // S1
    [Fact]
    public void SetActiveStatus_DraftStatusWithValidData_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, _event.status);
    }
    
    // S2
    [Fact]
    public void SetActiveStatus_ReadyStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        _event.SetReadyStatus(_defaultTime);
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, _event.status);
    }
    
    // S3
    [Fact]
    public void SetActiveStatus_AlreadyActive_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        _event.SetActiveStatus(_defaultTime);
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, _event.status);
    }
    
    // F2
    [Fact]
    public void SetActiveStatus_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        _event.SetCancelledStatus();
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventActiveStatus.CancelledEventCannotBeActivated);
    }
    
    // F1
    [Fact]
    public void SetActiveStatus_DraftMissingTitle_Failure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set everything except title (remains default)
        var description = EventDescription.Create("Valid description").Value;
        _event.UpdateDescription(description);
        
        var startTime = new DateTime(2050, 1, 2, 12, 0, 0);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TitleIsDefault);
    }
    
    // F1
    [Fact]
    public void SetActiveStatus_DraftMissingTimes_Failure()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set everything except times
        var title = EventTitle.Create("Valid Title").Value;
        _event.UpdateTitle(title);
        
        var description = EventDescription.Create("Valid description").Value;
        _event.UpdateDescription(description);
        
        // Act
        var result = _event.SetActiveStatus(_defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.TimesNotSet);
    }
    
    // F1
    [Fact]
    public void SetActiveStatus_EventInPast_Failure()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        
        // Set the current time to be after the event time
        var futureTime = new StubCurrentTime(new DateTime(2050, 1, 3, 12, 0, 0)); // After the event start
        
        // Act
        var result = _event.SetActiveStatus(futureTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventReadyStatus.StartTimeInPast);
    }

    #endregion
}