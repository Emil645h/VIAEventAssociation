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

public class DeclineInviteTests
{
      private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2026, 1, 1, 12, 0, 0));


    private readonly Guest _guest;
    
    public DeclineInviteTests()
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
    public void DeclineInviteTests_WithValidEventAndGuest_ReturnsSuccess()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        

        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        
        // Act
        var result = invite.RejectInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InviteStatus.Rejected, invite.inviteStatus);
    }
    
    // S2
    [Fact]
    public void DeclineInviteTests_WithAcceptedInvitation_ReturnsSuccess()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        

        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        invite.AcceptInvite(inviteId, _guest.Id, evt);
        
        // Act
        invite.RejectInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.Equal(InviteStatus.Rejected, invite.inviteStatus);
    }
    
    // F1
    [Fact]
    public void DeclineInviteTests_InvitationNotFound_ReturnsAccess()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var testGuestId = GuestId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, testGuestId).Value;

        

        invite.AssignToInvite(testGuestId, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        
        // Act
        var result = invite.RejectInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.Mismatch);
    }
    
    // F2
    [Fact]
    public void DeclineInviteTests_EventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        

        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        
        // Act
        evt.SetCancelledStatus();
        var result = invite.RejectInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToEventId.EventNotActive);
    }
}