using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.EventMethodTests;

public class EventTimeTests
{
    private readonly Event _event;
    
    public EventTimeTests()
    {
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        _event = Event.Create(eventId).Value;
    }

    // Success scenario tests
    // S1
    [Theory]
    [InlineData("2023-08-25 19:00", "2023-08-25 23:59")]
    [InlineData("2023-08-25 12:00", "2023-08-25 16:30")]
    [InlineData("2023-08-25 08:00", "2023-08-25 12:15")]
    [InlineData("2023-08-25 10:00", "2023-08-25 20:00")]
    [InlineData("2023-08-25 13:00", "2023-08-25 23:00")]
    public void Update_ValidTimesSameDay_ReturnsEventTime(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
        var eventTime = EventTime.Create(startTime, endTime).Value;

        // Act
        var result = _event.UpdateTime(eventTime);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(startTime, _event.eventTime.StartTime);
        Assert.Equal(endTime, _event.eventTime.EndTime);
    }

    // S2
    [Theory]
    [InlineData("2023-08-25 19:00", "2023-08-26 01:00")]
    [InlineData("2023-08-25 12:00", "2023-08-25 16:30")]
    [InlineData("2023-08-25 08:00", "2023-08-25 12:15")]
    public void Update_ValidTimesDifferentDays_ReturnsEventTime(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        
        // Act
        var result = _event.UpdateTime(eventTime);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(startTime, _event.eventTime.StartTime);
        Assert.Equal(endTime, _event.eventTime.EndTime);
    }
    
    // S3, S4, S5
    [Fact]
    public void Update_ReadyStatus_ChangesToDraft()
    {
        // Arrange
        _event.SetReadyStatus();
        var startTime = DateTime.Now.AddDays(1).Date.AddHours(10);
        var endTime = startTime.AddHours(10);
        var eventTime = EventTime.Create(startTime, endTime).Value;

        // Act
        var result = _event.UpdateTime(eventTime);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Draft, _event.status);
    }
    
    // Failure scenario tests
    
    // F1
    [Theory]
    [InlineData("2023-08-26 19:00", "2023-08-25 01:00")]
    [InlineData("2023-08-26 19:00", "2023-08-25 23:59")]
    [InlineData("2023-08-27 12:00", "2023-08-25 16:30")]
    [InlineData("2023-08-01 08:00", "2023-07-31 12:15")]
    public void Update_StartDateAfterEndDate_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
        
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartDateAfterEndDate);
    }
    
    // F2
    [Theory]
    [InlineData("2023-08-26 19:00", "2023-08-26 14:00")]
    [InlineData("2023-08-26 16:00", "2023-08-26 00:00")]
    [InlineData("2023-08-26 19:00", "2023-08-26 18:59")]
    [InlineData("2023-08-26 12:00", "2023-08-26 10:10")]
    [InlineData("2023-08-26 08:00", "2023-08-26 00:30")]
    public void Update_StartTimeAfterEndTime_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
    
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartTimeAfterEndTime);
    }
    
    // F3
    [Theory]
    [InlineData("2023-08-26 14:00", "2023-08-26 14:50")]
    [InlineData("2023-08-26 18:00", "2023-08-26 18:59")]
    [InlineData("2023-08-26 12:00", "2023-08-26 12:30")]
    [InlineData("2023-08-26 08:00", "2023-08-26 08:00")]
    public void Update_EventDurationTooShort_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
    
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.DurationTooShort);
    }
    
    // F4, F9
    [Theory]
    [InlineData("2023-08-30 08:00", "2023-08-30 18:01")]
    [InlineData("2023-08-30 14:59", "2023-08-31 01:00")]
    [InlineData("2023-08-30 14:00", "2023-08-31 00:01")]
    public void Update_EventDurationTooLong_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
    
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.DurationTooLong);
    }
    
    // F5
    [Theory]
    [InlineData("2023-08-25 07:50", "2023-08-25 14:00")]
    [InlineData("2023-08-25 07:59", "2023-08-25 15:00")]
    [InlineData("2023-08-25 01:01", "2023-08-25 08:30")]
    public void Update_StartTimeTooEarly_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
        
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartTimeTooEarly);
    }
    
    // F6
    [Theory]
    [InlineData("2023-08-24 23:50", "2023-08-25 01:01")]
    [InlineData("2023-08-24 22:00", "2023-08-25 07:59")]
    [InlineData("2023-08-30 23:00", "2023-08-31 02:30")]
    public void Update_EndTimeTooLate_ReturnsFailure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
        
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.EndTimeTooLate);
    }
    
    // F7
    [Fact]
    public void Update_ActiveEvent_ReturnsFailure()
    {
        // Arrange
        _event.SetActiveStatus();
        var startTime = DateTime.Now.AddDays(1).Date.AddHours(10);
        var endTime = startTime.AddHours(4);
        var eventTime = EventTime.Create(startTime, endTime).Value;
    
        // Act
        var result = _event.UpdateTime(eventTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.ActiveEventCannotBeModified);
    }
    
    // F8
    [Fact]
    public void Update_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        _event.SetCancelledStatus();
        var startTime = DateTime.Now.AddDays(1).Date.AddHours(10);
        var endTime = startTime.AddHours(4);
        var eventTime = EventTime.Create(startTime, endTime).Value;
    
        // Act
        var result = _event.UpdateTime(eventTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
    
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.CancelledEventCannotBeModified);
    }
    
    // F10
    [Fact]
    public void Update_StartTimeInPast_ReturnsFailure()
    {
        // Arrange
        var startTime = DateTime.Now.AddHours(-1); // In the past
        var endTime = DateTime.Now.AddHours(3);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
    
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartTimeInPast);
    }
    
    // F11
    [Theory]
    [InlineData("2023-08-31 00:30", "2023-08-31 08:30")]
    [InlineData("2023-08-30 23:59", "2023-08-31 08:01")]
    [InlineData("2023-08-31 01:00", "2023-08-31 08:00")]
    public void UpdateTime_SpansInvalidTime_Failure(string startTimeStr, string endTimeStr)
    {
        // Arrange
        var startTime = DateTime.Parse(startTimeStr).AddYears(10); // Ensure it's in the future
        var endTime = DateTime.Parse(endTimeStr).AddYears(10);
    
        // Act
        var eventTimeResult = EventTime.Create(startTime, endTime);
        var resultFailure = Assert.IsType<Failure<EventTime>>(eventTimeResult);
    
        // Assert
        Assert.True(eventTimeResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.InvalidTimeSpan);
    }
}