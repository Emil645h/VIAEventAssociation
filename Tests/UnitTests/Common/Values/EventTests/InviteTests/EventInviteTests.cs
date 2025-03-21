using System;
using UnitTests.Fakes;
using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class EventInviteTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));
    private readonly Guest _guest;
    
    public EventInviteTests()
    {
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("Emil").Value;
        var lastName = LastName.Create("Brugge").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePicUrl = ProfilePictureUrl.Create("https://handsome.guy/image.jpg").Value;
        _guest = Guest.Create(guestId, firstName, lastName, email, profilePicUrl).Value;
    }
    
    private Event CreateValidReadyEvent()
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
        
        // Set ready status
        _event.SetReadyStatus(_defaultTime);
        
        return _event;
    }
    
    private Event CreateValidActiveEvent()
    {
        var _event = CreateValidReadyEvent();
        _event.SetActiveStatus(_defaultTime);
        return _event;
    }
    
    private Event CreateValidDraftEvent()
    {
        // Create a valid event in draft status
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
        
        return _event;
    }
    
    private Event CreateEventWithParticipant()
    {
        var _event = CreateValidActiveEvent();
        _event.MakePublic();
        _event.AssignGuestToGuestList(_guest, _defaultTime);
        return _event;
    }
    
    private Event CreateFullEvent()
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
    public void Create_ReadyEvent_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidReadyEvent();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void Create_ActiveEvent_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidActiveEvent();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void Create_DraftEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftEvent();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.Invite.EventNotReadyOrActive);
    }
    
    [Fact]
    public void Create_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidReadyEvent();
        _event.SetCancelledStatus();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.Invite.EventNotReadyOrActive);
    }
    
    [Fact]
    public void Create_NoMoreRoom_ReturnsFailure()
    {
        // Arrange
        var _event = CreateFullEvent();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.Invite.EventIsFull);
    }
    
    [Fact]
    public void Create_GuestAlreadyInvited_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidReadyEvent();
        
        // First invite
        _event.CreateGuestInvite(_guest);
        
        // Act - Second invite to the same guest
        var result = _event.CreateGuestInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.Invite.GuestIsAlreadyInvited);
    }
    
    [Fact]
    public void Create_GuestAlreadyParticipating_ReturnsFailure()
    {
        // Arrange
        var _event = CreateEventWithParticipant();
        
        // Act
        var result = _event.CreateGuestInvite(_guest);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.Invite.GuestIsAlreadyParticipating);
    }
}