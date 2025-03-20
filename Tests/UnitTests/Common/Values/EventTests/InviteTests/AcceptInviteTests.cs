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

public class AcceptInviteTests
{
    
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2026, 1, 1, 12, 0, 0));
    private readonly ICurrentTime _defaultPastTime = new StubCurrentTime(new DateTime(2024, 1, 1, 12, 0, 0));



    private readonly Guest _guest;
    
    public AcceptInviteTests()
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
    
    private Event CreateValidReadyPublicEvent()
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
        _event.SetReadyStatus(_defaultTime);
        
        return _event;
    }
    
    
    
    // S1
    [Fact]
    public void AcceptInvite_GivenValidEvent_ReturnsSuccess()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();

        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        

        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);

        // Act
        var result = invite.AcceptInvite(inviteId, _guest.Id, evt);

        // Assert
        Assert.True(result.IsSuccess, "Invitation acceptance should succeed.");
        Assert.Equal(InviteStatus.Accepted, invite.inviteStatus);
    }
    
    // F1
    [Fact]
    public void AcceptInvite_GivenNoInvitationForGuest_ReturnsFailure()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();

        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var testGuestId = GuestId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, testGuestId).Value;
        
        Assert.NotEqual(_guest.Id, invite.assignedGuestId);
        
        // Act
        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        var result = invite.AcceptInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsFailure, "Invitation acceptance should fail.");
    }
    
    // F2
    [Fact]
    public void AcceptInvite_TooManyGuests_ReturnsFailure()
    {
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        
        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        
        for (int i = 1; i < 6; i++)
        {
            var guestId = GuestId.Create(Guid.NewGuid());
            var firstName = FirstName.Create($"Guest");
            var lastName = LastName.Create("Test");
            var email = ViaEmail.Create($"12345{i}@via.dk");
            var profilePicUrl = ProfilePictureUrl.Create("https://someurl.com/profile.jpg");
            var guest = Guest.Create(guestId.Value, firstName.Value, lastName.Value, email.Value, profilePicUrl.Value);

            var assignResult = evt.AssignGuestToGuestList(guest.Value, _defaultTime);
            Assert.True(assignResult.IsSuccess, $"Guest {i} should be successfully assigned. EventStatus: {evt.status}, GuestCount: {evt.guestList.numberOfGuests}/{evt.maxGuests.Value}");
        }
        
        //Act
        var result = invite.AcceptInvite(inviteId, _guest.Id, evt);
        
        //Assert
        Assert.Equal(5, evt.guestList.numberOfGuests);
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToEventId.EventIsFull);
    }
    
    // F3
    [Fact]
    public void AcceptInviteTests_EventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var evt = CreateValidActivePublicEvent();
        
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        
        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        
        // Act
        evt.SetCancelledStatus();
        var result = invite.AcceptInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsFailure, "Invitation acceptance should fail.");
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToEventId.EventNotActive);
    }
    
    // F4
    [Fact]
    public void AcceptInviteTests_EventIsReady_ReturnsFailure()
    {
        // Arrange
        var evt = CreateValidReadyPublicEvent();
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId, _guest.Id).Value;
        
        // Act
        invite.AssignToInvite(_guest.Id, evt.status, evt.guestList.numberOfGuests, evt.maxGuests.Value, new List<GuestId>(), evt.guestList);
        var result = invite.AcceptInvite(inviteId, _guest.Id, evt);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.True(evt.status == EventStatus.Ready);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToEventId.EventNotActive);
    }


}