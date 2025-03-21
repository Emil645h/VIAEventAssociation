using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class EventRejectInviteTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2023, 1, 1, 12, 0, 0));
    private readonly Guest _guest;

    public EventRejectInviteTests()
    {
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("Emil").Value;
        var lastName = LastName.Create("Brugge").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePicUrl = ProfilePictureUrl.Create("https://handsome.guy/image.jpg").Value;
        _guest = Guest.Create(guestId, firstName, lastName, email, profilePicUrl).Value;
    }

    private Event CreateActiveEventWithInvite()
    {
        // Create a valid event
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;

        // Set valid title
        var title = EventTitle.Create("Valid Event").Value;
        _event.UpdateTitle(title);

        // Set valid description
        var description = EventDescription.Create("A valid event for testing").Value;
        _event.UpdateDescription(description);

        // Set valid time (in the future)
        var startTime = _defaultTime.GetCurrentTime().AddDays(1);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime, _defaultTime).Value;
        _event.UpdateTime(eventTime);

        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);

        // Make it active
        _event.SetReadyStatus(_defaultTime);
        _event.SetActiveStatus(_defaultTime);

        // Create an invite
        _event.CreateGuestInvite(_guest);

        return _event;
    }

    private Event CreateActiveEventWithAcceptedInvite()
    {
        // Create an event with an invitation
        var _event = CreateActiveEventWithInvite();

        // Accept the invitation
        _event.GuestAcceptsInvite(_guest, _defaultTime);

        return _event;
    }

    private Event CreateCancelledEventWithInvite()
    {
        // Create an active event with invite
        var _event = CreateActiveEventWithInvite();

        // Cancel the event
        _event.SetCancelledStatus();

        return _event;
    }

    [Fact]
    public void DeclineInvite_PendingInvite_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();

        // Act
        var result = _event.GuestRejectsInvite(_guest);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify the invite status changed by trying to accept it (should fail)
        var acceptResult = _event.GuestAcceptsInvite(_guest, _defaultTime);
        Assert.True(acceptResult.IsFailure);
    }
    
    [Fact]
    public void DeclineInvite_AcceptedInvite_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateActiveEventWithAcceptedInvite();
    
        // Act
        var result = _event.GuestRejectsInvite(_guest);
    
        // Assert
        Assert.True(result.IsSuccess);
    
        // Verify the invite status changed by trying to accept it again (should fail)
        var acceptResult = _event.GuestAcceptsInvite(_guest, _defaultTime);
        Assert.True(acceptResult.IsFailure);
    }
    
    [Fact]
    public void DeclineInvite_InvitationNotFound_ReturnsFailure()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();
        var nonInvitedGuestId = GuestId.Create(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("Steve").Value;
        var lastName = LastName.Create("Jobs").Value;
        var email = ViaEmail.Create("123456@via.dk").Value;
        var profilePic = ProfilePictureUrl.Create("https://nice.com/image.jpg").Value;
        var nonInvitedGuest = Guest.Create(nonInvitedGuestId, firstName, lastName, email, profilePic).Value;
    
        // Act
        var result = _event.GuestRejectsInvite(nonInvitedGuest);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.InvitationNotFound);
    }
    
    [Fact]
    public void DeclineInvite_EventCancelled_ReturnsFailure()
    {
        // Arrange
        var _event = CreateCancelledEventWithInvite();
    
        // Act
        var result = _event.GuestRejectsInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.EventCancelled);
    }
    
    [Fact]
    public void DeclineInvite_AlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();
        _event.GuestRejectsInvite(_guest);
    
        // Act
        var result = _event.GuestRejectsInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.RejectInvite.AlreadyRejected);
    }
}