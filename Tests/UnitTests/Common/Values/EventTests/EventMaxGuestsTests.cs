using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventMaxGuestsTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));
    
    private Event CreateEvent(EventStatus status, int currentMaxGuests = 10)
    {
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;

        // Set valid title
        var title = EventTitle.Create("Valid Title").Value;
        _event.UpdateTitle(title);
        
        // Set valid time (in the future)
        var startTime = new DateTime(2050, 1, 2, 12, 0, 0);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime, _defaultTime).Value;
        _event.UpdateTime(eventTime);
        
        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Set the desired status
        if (status.Equals(EventStatus.Ready))
        {
            _event.SetReadyStatus(_defaultTime);
        }
        else if (status.Equals(EventStatus.Active))
        {
            _event.SetActiveStatus(_defaultTime);
        }
        else if (status.Equals(EventStatus.Cancelled))
        {
            _event.SetCancelledStatus();
        }

        return _event;
    }

    // S1
    [Fact]
    public void Update_DraftStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft);
        var newMaxGuests = EventMaxGuests.Create(25).Value;

        // Act
        var result = _event.UpdateMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, _event.maxGuests);
        Assert.Equal(EventStatus.Draft, _event.status);
    }

    // S2
    [Fact]
    public void Update_ReadyStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Ready);
        var newMaxGuests = EventMaxGuests.Create(25).Value;

        // Act
        var result = _event.UpdateMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, _event.maxGuests);
        Assert.Equal(EventStatus.Ready, _event.status);
    }

    // S3
    [Fact]
    public void Update_ActiveStatusAndIncrease_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Active);
        int initialValue = _event.maxGuests.Value;
        int newMaxGuestsValue = initialValue + 10; // Increase by 10
        var newMaxGuests = EventMaxGuests.Create(newMaxGuestsValue).Value;
        
        // Act
        var result = _event.UpdateMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, _event.maxGuests);
        Assert.Equal(EventStatus.Active, _event.status);
    }

    // F1
    [Fact]
    public void Update_ActiveStatusAndDecrease_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Active);
        int initialValue = _event.maxGuests.Value;
        int newMaxGuestsValue = initialValue - 1; // Decrease by 1
        // Create the new EventMaxGuests object with initialValue - 1 as value.
        var newMaxGuests = EventMaxGuests.Create(newMaxGuestsValue).Value;

        // Act
        var result = _event.UpdateMaxGuests(newMaxGuests);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.CannotBeReduced);
        Assert.Equal(initialValue, _event.maxGuests.Value);
    }

    // F2
    [Fact]
    public void Update_CancelledStatus_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Cancelled);
        int newMaxGuestsValue = 20;
        var newMaxGuests = EventMaxGuests.Create(newMaxGuestsValue).Value;

        // Act
        var result = _event.UpdateMaxGuests(newMaxGuests);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.CancelledEventCannotBeModified);
    }

    // F4
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void Update_TooSmall_ReturnsFailure(int newMaxGuestsValue)
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft);

        // Act
        var result = EventMaxGuests.Create(newMaxGuestsValue);
        var resultFailure = Assert.IsType<Failure<EventMaxGuests>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.TooSmall);
    }

    // F5
    [Theory]
    [InlineData(51)]
    [InlineData(100)]
    public void Update_TooLarge_ReturnsFailure(int newMaxGuestsValue)
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft);

        // Act
        var result = EventMaxGuests.Create(newMaxGuestsValue);
        var resultFailure = Assert.IsType<Failure<EventMaxGuests>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventMaxGuests.TooLarge);
    }

    // TODO: Test for location capacity is not implemented yet (F3)
    // This will be added when we have the Location functionality
}