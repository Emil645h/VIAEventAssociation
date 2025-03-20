using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;
namespace UnitTests.Common.Values.EventTests.InviteTests;

public class InviteGuestTest
{
    
    /* Vi bør overveje om hvorvidt muligt et event skal holde på en invite-list, lidt a la guest-list */

    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2026, 1, 1, 12, 0, 0));
    private readonly Guest _guest;
    
    public InviteGuestTest()
    {
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("Emil").Value;
        var lastName = LastName.Create("Brugge").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePicUrl = ProfilePictureUrl.Create("https://handsome.guy/image.jpg").Value;
        _guest = Guest.Create(guestId, firstName, lastName, email, profilePicUrl).Value;
    }
    
    private Event CreateValidActivePublicEvent()
    {
        // Create a valid event
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var _event = Event.Create(eventId).Value;
        
        // Set valid title
        var title = EventTitle.Create("Valid Public Event").Value;
        _event.UpdateTitle(title);
        
        // Set valid description
        var description = EventDescription.Create("A valid public event for testing").Value;
        _event.UpdateDescription(description);
        
        // Set valid time (in the future)
        var startTime = _defaultTime.GetCurrentTime().AddHours(1);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        
        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(5).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Make it public
        _event.MakePublic();
        
        // Set active status
        _event.SetActiveStatus(_defaultTime);
        
        return _event;
    }

    // S1
    [Fact]
    public void InviteGuestTest_WithValidGuestId_ReturnsSuccess()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        var invitedGuests = new List<GuestId>();
        
        
        // Act
        var result = invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, invitedGuests, evt.guestList);

        // Assert
        Assert.True(result.IsSuccess);
    }




    // F1
    [Fact]
    public void InviteGuestTest_WhenEventIsDraftOrCancelled_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;

        var guestId = GuestId.Create(Guid.NewGuid()).Value;

        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(eventId).Value;
        var invitedGuests = new List<GuestId>();

        // Act
        var result = invite.AssignToInvite(guestId, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, invitedGuests,evt.guestList);

        // Assert
        Assert.True(result.IsFailure);
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, error => error.Code == InviteErrors.AssignToEventId.EventNotActive.Code);
    }
    
    // F2
    [Fact] 
    public void InviteGuestTest_WhenEventIsFull_ReturnsFailure() 
    {
    // Arrange
    var evt = CreateValidActivePublicEvent();
    
    var inviteId = InviteId.Create(Guid.NewGuid()).Value;
    var invite = Invite.Create(inviteId, _guest.Id).Value;
    var invitedGuests = new List<GuestId>();
    

 
    for (int i = 1; i < 6; i++)
    {
        var guestId = GuestId.Create(Guid.NewGuid());
        var firstName = FirstName.Create($"Guest");
        var lastName = LastName.Create("Test");
        var email = ViaEmail.Create($"12345{i}@via.dk");
        var profilePicUrl = ProfilePictureUrl.Create("https://someurl.com/profile.jpg");
        var guest = Guest.Create(guestId.Value, firstName.Value, lastName.Value, email.Value, profilePicUrl.Value);
        
        evt.AssignGuestToGuestList(guest.Value, _defaultTime);
    }

    Assert.Equal(evt.maxGuests.Value, evt.guestList.numberOfGuests);

    // Act
    var result = invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, invitedGuests, evt.guestList);

    // Assert
    Assert.True(result.IsFailure, "Expected failure when inviting a guest to a full event.");
    var failure = Assert.IsType<Failure<None>>(result);
    Assert.Contains(failure.Errors, error => error.Code == InviteErrors.AssignToEventId.EventIsFull.Code);
}

// F3
    [Fact]
    public void InviteGuestTest_WhenGuestIsAlreadyInvited_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;

        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var eventTitle = EventTitle.Create("Test Event").Value;
        var eventTime = EventTime.Create(new DateTime(2025, 4, 1, 12, 0, 0), new DateTime(2025, 4, 1, 15, 0, 0)).Value;
        var maxGuests = EventMaxGuests.Create(5).Value;
        var invitedGuests = new List<GuestId>();
        invitedGuests.Add(_guest.Id);

        var evt = Event.Create(eventId).Value;
        evt.UpdateTitle(eventTitle);
        evt.UpdateTime(eventTime);
        evt.UpdateMaxGuests(maxGuests);
        evt.MakePublic();
        evt.SetActiveStatus(_defaultTime);
        evt.AssignGuestToGuestList(_guest, _defaultTime);

        // Act
        var result = invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, invitedGuests, evt.guestList);

        // Assert
        Assert.True(result.IsFailure, "Expected failure when inviting an already invited guest.");
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, error => error.Code == InviteErrors.AssignToEventId.GuestIsAlreadyInvited.Code);
    }
    
    // F4
    [Fact]
    public void InviteGuestTest_GuestIsAlreadyParticipating_ReturnsFailure()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        var invitedGuests = new List<GuestId>();
        

        //
        var assignGuestResult = evt.AssignGuestToGuestList(_guest, _defaultTime);
        Assert.True(assignGuestResult.IsSuccess, "Guest should be successfully assigned as a participant.");

        // Act
        var result = invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, invitedGuests,evt.guestList);
    
        // Assert
        Assert.True(result.IsFailure, "Expected failure when inviting a guest that is already participating.");
        Assert.Equal(1, evt.guestList.numberOfGuests);
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, error => error == InviteErrors.AssignToEventId.GuestIsAlreadyParticipating);
    }




}