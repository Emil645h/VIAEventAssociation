using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.EventMethodTests;

public class EventDescriptionTests
{
    // S1: Draft event with valid description (0-250 chars)
    [Fact]
    public void Update_WhenEventInDraftStatus_ShouldUpdateDescription()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        var newDescription = EventDescription.Create("Updated Description").Value;

        // Act
        var result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, evt.description);
        Assert.Equal(EventStatus.Draft, evt.status);
    }

    // S2: Setting description to empty
    [Fact]
    public void Update_WhenSettingToEmpty_ShouldSetEmptyDescription()
    {
        // Arrange
        var initialDesc = EventDescription.Create("Initial description").Value;
        var newDescription = EventDescription.Create("").Value;
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        evt.description = initialDesc;
        Assert.Equal(initialDesc, evt.description);

        // Act

        var result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", evt.description.Value);
        Assert.Equal(EventStatus.Draft, evt.status);
    }

    // S3: Ready event gets updated and changes to Draft
    [Fact]
    public void Update_WhenEventInReadyStatus_ShouldUpdateAndChangeToDraft()
    {
        // Arrange
        var initialDesc = EventDescription.Create("Initial description").Value;
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        evt.description = initialDesc;
        evt.status = EventStatus.Ready;
        Assert.Equal(initialDesc, evt.description);
        Assert.Equal(EventStatus.Ready, evt.status);
        var newDescription = EventDescription.Create("Updated Description").Value;
        // Act
        var result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, evt.description);
        Assert.Equal(EventStatus.Draft, evt.status);
    }

    // F1: Description too long (>250 chars)
    [Fact]
    public void Update_WhenDescriptionTooLong_ShouldFailWithMessage()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;

        // Create a 251-character string
        var tooLongDescription = new string('x', 251);

        // Act
        var result = EventDescription.Create(tooLongDescription);

        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<EventDescription>>(result);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventDescription.InvalidCharacterLength);
    }

    // F2: Cancelled events cannot be modified
    [Fact]
    public void Update_WhenEventIsCancelled_ShouldFailWithMessage()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        evt.status = EventStatus.Cancelled;
        var newDescription = EventDescription.Create("Updated Description").Value;

        // Act
        var result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventDescription.InvalidEventStatus);
    }

    // F3: Active events cannot be modified
    [Fact]
    public void Update_WhenEventIsActive_ShouldFailWithMessage()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        evt.status = EventStatus.Active;
        var newDescription = EventDescription.Create("Updated Description").Value;

        // Act
        var result = evt.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventDescription.InvalidEventStatus);
    }
}