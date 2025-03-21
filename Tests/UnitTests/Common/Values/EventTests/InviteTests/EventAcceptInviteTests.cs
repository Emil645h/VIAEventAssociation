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

public class EventAcceptInviteTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));
    private readonly Guest _guest;

    public EventAcceptInviteTests()
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

    private Event CreateReadyEventWithInvite()
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

        // Make it ready
        _event.SetReadyStatus(_defaultTime);

        // Create an invite
        _event.CreateGuestInvite(_guest);

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

    private Event CreateFullActiveEventWithInvite()
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

        _event.SetActiveStatus(_defaultTime);
        _event.MakePublic();

        _event.CreateGuestInvite(_guest);

        // Fill the event with participants
        for (int i = 0; i < 5; i++)
        {
            var guestId = GuestId.Create(Guid.NewGuid()).Value;
            var firstName = FirstName.Create("Emil").Value;
            var lastName = LastName.Create("Brugge").Value;
            var email = ViaEmail.Create("331458@via.dk").Value;
            var profilePicUrl = ProfilePictureUrl.Create("https://handsome.guy/image.jpg").Value;
            var guest = Guest.Create(guestId, firstName, lastName, email, profilePicUrl).Value;
            _event.AssignGuestToGuestList(guest, _defaultTime);
        }

        return _event;
    }

    [Fact]
    public void AcceptInvite_ValidInvite_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();

        // Act
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AcceptInvite_InvitationNotFound_ReturnsFailure()
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
        var result = _event.GuestAcceptsInvite(nonInvitedGuest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.InvitationNotFound);
    }

    [Fact]
    public void AcceptInvite_EventFull_ReturnsFailure()
    {
        // Arrange
        var _event = CreateFullActiveEventWithInvite();

        // Act
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.EventFull);
    }

    [Fact]
    public void AcceptInvite_EventCancelled_ReturnsFailure()
    {
        // Arrange
        var _event = CreateCancelledEventWithInvite();

        // Act
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.EventCancelled);
    }

    [Fact]
    public void AcceptInvite_EventReady_ReturnsFailure()
    {
        // Arrange
        var _event = CreateReadyEventWithInvite();

        // Act
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.EventNotActive);
    }

    [Fact]
    public void AcceptInvite_EventInPast_ReturnsFailure()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();

        // Create a future time where the event has passed
        var futureTime = new StubCurrentTime(_defaultTime.GetCurrentTime().AddDays(2));

        // Act
        var result = _event.GuestAcceptsInvite(_guest, futureTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.Invitation.EventInPast);
    }

    [Fact]
    public void AcceptInvite_AlreadyAccepted_ReturnsFailure()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();

        // Accept the invite once
        _event.GuestAcceptsInvite(_guest, _defaultTime);

        // Act - Try to accept again
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AcceptInvite.AlreadyAccepted);
    }

    [Fact]
    public void AcceptInvite_AlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var _event = CreateActiveEventWithInvite();

        // Accept the invite once
        _event.GuestRejectsInvite(_guest);

        // Act - Try to accept again
        var result = _event.GuestAcceptsInvite(_guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AcceptInvite.InvitationRejected);
    }
}