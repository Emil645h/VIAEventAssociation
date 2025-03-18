using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventVisibilityTests
{
    private Event CreateEvent(EventStatus status, EventVisibility visibility)
    {
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;

        if (visibility.Equals(EventVisibility.Public))
        {
            _event.MakePublic();
        }
        
        if (status.Equals(EventStatus.Ready))
        {
            _event.SetReadyStatus();
        }
        else if (status.Equals(EventStatus.Active))
        {
            _event.SetActiveStatus();
        }
        else if (status.Equals(EventStatus.Cancelled))
        {
            _event.SetCancelledStatus();
        }

        return _event;
    }

    #region Make Public

    // UC 5, S1
    [Fact]
    public void MakePublic_DraftStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft, EventVisibility.Private);

        // Act
        var result = _event.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, _event.visibility);
        Assert.Equal(EventStatus.Draft, _event.status);
    }

    // UC 5, S1
    [Fact]
    public void MakePublic_ReadyStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Ready, EventVisibility.Private);

        // Act
        var result = _event.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, _event.visibility);
        Assert.Equal(EventStatus.Ready, _event.status);
    }

    // UC 5, S1
    [Fact]
    public void MakePublic_ActiveStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Active, EventVisibility.Private);

        // Act
        var result = _event.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, _event.visibility);
        Assert.Equal(EventStatus.Active, _event.status);
    }

    // UC 5, F1
    [Fact]
    public void MakePublic_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Cancelled, EventVisibility.Private);

        // Act
        var result = _event.MakePublic();
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.CancelledEventCannotBeModified);

        // The visibility should not have changed
        Assert.Equal(EventVisibility.Private, _event.visibility);
    }

    #endregion
    
    #region Make Private

    // UC 6, S1
    [Fact]
    public void MakePrivate_AlreadyPrivateDraftStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft, EventVisibility.Private);

        // Act
        var result = _event.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, _event.visibility);
        Assert.Equal(EventStatus.Draft, _event.status);
    }

    // UC 6, S1
    [Fact]
    public void MakePrivate_AlreadyPrivateReadyStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Ready, EventVisibility.Private);

        // Act
        var result = _event.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, _event.visibility);
        Assert.Equal(EventStatus.Ready, _event.status);
    }

    // UC 6, S2
    [Fact]
    public void MakePrivate_PublicDraftStatus_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Draft, EventVisibility.Public);

        // Act
        var result = _event.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, _event.visibility);
        Assert.Equal(EventStatus.Draft, _event.status);
    }

    // UC 6, S2
    [Fact]
    public void MakePrivate_PublicReadyStatus_StatusChangesToDraft()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Ready, EventVisibility.Public);

        // Act
        var result = _event.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, _event.visibility);
        Assert.Equal(EventStatus.Draft, _event.status); // Status changes to draft
    }

    // UC 6, F1
    [Fact]
    public void MakePrivate_ActiveEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Active, EventVisibility.Public);

        // Act
        var result = _event.MakePrivate();
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.ActiveEventCannotBePrivate);

        // The visibility should not have changed
        Assert.Equal(EventVisibility.Public, _event.visibility);
        Assert.Equal(EventStatus.Active, _event.status);
    }

    // UC 6, F2
    [Fact]
    public void MakePrivate_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEvent(EventStatus.Cancelled, EventVisibility.Public);

        // Act
        var result = _event.MakePrivate();
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventVisibility.CancelledEventCannotBeModified);

        // The visibility should not have changed
        Assert.Equal(EventVisibility.Public, _event.visibility);
        Assert.Equal(EventStatus.Cancelled, _event.status);
    }

    #endregion
}