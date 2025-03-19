using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventParticipationTests : IDisposable
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));

    private readonly Guest guest;
    
    public EventParticipationTests()
    {
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("Emil").Value;
        var lastName = LastName.Create("Brugge").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePicUrl = ProfilePictureUrl.Create("https://handsome.guy/image.jpg").Value;
        guest = Guest.Create(guestId, firstName, lastName, email, profilePicUrl).Value;
    }
    
    public void Dispose()
    {
        // Nothing
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
        var startTime = _defaultTime.GetCurrentTime().AddDays(1);
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
    
    private Event CreateValidDraftPublicEvent()
    {
        var _event = CreateValidActivePublicEvent();
        // Use reflection or a test-only method to set status back to draft
        // For simplicity, we'll recreate it without activating
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        _event = Event.Create(eventId).Value;
        
        // Set valid title
        var title = EventTitle.Create("Valid Public Event").Value;
        _event.UpdateTitle(title);
        
        // Set valid description
        var description = EventDescription.Create("A valid public event for testing").Value;
        _event.UpdateDescription(description);
        
        // Set valid time (in the future)
        var startTime = _defaultTime.GetCurrentTime().AddDays(1);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime).Value;
        _event.UpdateTime(eventTime);
        
        // Set valid max guests
        var maxGuests = EventMaxGuests.Create(20).Value;
        _event.UpdateMaxGuests(maxGuests);
        
        // Make it public
        _event.MakePublic();
        
        return _event;
    }
    
    private Event CreateValidDraftPrivateEvent()
    {
        var _event = CreateValidDraftPublicEvent();
        // Make it private
        _event.MakePrivate();
        
        return _event;
    }
    
    private Event CreateFullActivePublicEvent()
    {
        // Create a valid active public event
        var _event = CreateValidActivePublicEvent();
        
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

    #region Assign
    
    // UC 11, S1
    [Fact]
    public void AssignGuestToGuestList_ValidEvent_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        
        // Act
        var result = _event.AssignGuestToGuestList(guest, _defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, _event.guestList.numberOfGuests);
    }
    
    // UC 11, F1
    [Fact]
    public void AssignGuestToGuestList_EventNotActive_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftPublicEvent();
        
        // Act
        var result = _event.AssignGuestToGuestList(guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.Participation.EventNotActive);
    }
    
    // UC 11, F4
    [Fact]
    public void AssignGuestToGuestList_EventPrivate_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidDraftPrivateEvent();
        _event.SetActiveStatus(_defaultTime);
        
        // Act
        var result = _event.AssignGuestToGuestList(guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.Participation.EventIsPrivate);
    }
    
    // UC 11, F3
    [Fact]
    public void AssignGuestToGuestList_EventStarted_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        
        // Create a time after the event starts
        var futureTime = new StubCurrentTime(_defaultTime.GetCurrentTime().AddDays(2));
        
        // Act
        var result = _event.AssignGuestToGuestList(guest, futureTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.Participation.EventAlreadyStarted);
    }
    
    // UC 11, F2
    [Fact]
    public void AssignGuestToGuestList_NoMoreRoom_ReturnsFailure()
    {
        // Arrange
        var _event = CreateFullActivePublicEvent();
        
        // Act
        var result = _event.AssignGuestToGuestList(guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.Participation.NoMoreRoom);
    }
    
    // UC 11, F5
    [Fact]
    public void AssignGuestToGuestList_AlreadyParticipating_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        
        // First participation
        _event.AssignGuestToGuestList(guest, _defaultTime);
        
        // Act - Try to participate again
        var result = _event.AssignGuestToGuestList(guest, _defaultTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.AssignToGuestList.GuestAlreadyAssigned);
    }
    
    #endregion

    #region Remove

    // UC 12, S1
    [Fact]
    public void RemoveFromGuestList_ParticipatingGuest_ReturnsSuccess()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        _event.AssignGuestToGuestList(guest, _defaultTime);
        Assert.Equal(1, _event.guestList.numberOfGuests);
        
        // Act
        var result = _event.RemoveGuestFromGuestList(guest, _defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, _event.guestList.numberOfGuests);
    }
    
    // UC 12, S2
    [Fact]
    public void RemoveFromGuestList_NonParticipatingGuest_NoChange()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        Assert.Equal(0, _event.guestList.numberOfGuests);
        
        // Act
        var result = _event.RemoveGuestFromGuestList(guest, _defaultTime);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, _event.guestList.numberOfGuests);
    }
    
    // UC 12, F1
    [Fact]
    public void RemoveFromGuestList_EventStarted_ReturnsFailure()
    {
        // Arrange
        var _event = CreateValidActivePublicEvent();
        
        // Create a time after the event starts
        var futureTime = new StubCurrentTime(_defaultTime.GetCurrentTime().AddDays(2));
        
        // Act
        var result = _event.RemoveGuestFromGuestList(guest, futureTime);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.Participation.CannotCancelPastEvent);
    }

    #endregion
}