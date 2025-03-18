using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventTests
{
    //UC S1-S4
    [Fact]
    public void Create_EmptyEvent_ReturnsEvent()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;

        // Act
        var result = Event.Create(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.maxGuests.Value);
        Assert.Equal(EventVisibility.Private, result.Value.visibility);
        Assert.Equal(EventStatus.Draft, result.Value.status);
        Assert.Equal("Working Title", result.Value.title.Value);
        Assert.Equal("", result.Value.description.Value);
    }
    
    //UC2 - S1
    [Fact]
    public void Update_TitleOfEvent_ReturnsSuccess()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var eventResult = Event.Create(id);
        var newTitle = EventTitle.Create("New Title").Value;

        // Act
        var result = eventResult.Value.UpdateTitle(newTitle);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", eventResult.Value.title.Value);
    }
    
    //UC2 - S2
    [Fact]
    public void Update_TitleOfEventWhenEventIsReady_ReturnsSuccess()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var eventResult = Event.Create(id);
        eventResult.Value.SetReadyStatus();
        var newTitle = EventTitle.Create("New Title").Value;

        // Act
        var result = eventResult.Value.UpdateTitle(newTitle);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", eventResult.Value.title.Value);
        Assert.Equal(EventStatus.Draft, eventResult.Value.status);
    }
    
    
    //UC2 - F1
    [Fact]
    public void Update_TitleOfEventIsEmpty_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;

        // Act
        var newTitleResult = EventTitle.Create("");
        var resultFailure = Assert.IsType<Failure<EventTitle>>(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.TitleIsEmpty);
    }
    
    //UC2 - F2
    [Fact]
    public void Update_TitleOfEventIsTooShort_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;

        // Act
        var newTitleResult = EventTitle.Create("A");
        var resultFailure = Assert.IsType<Failure<EventTitle>>(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidTitleCharacterLimit);
    }
    
    //UC2 - F3
    [Fact]
    public void Update_TitleOfEventIsTooLong_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;
        

        // Act
        var newTitleResult = EventTitle.Create(new string('a', 76));
        var resultFailure = Assert.IsType<Failure<EventTitle>>(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidTitleCharacterLimit);
    }
    
    //UC2 - F4
    [Fact]
    public void Update_TitleOfEventIsNull_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;
        
        // Act
        var newTitleResult = EventTitle.Create(null!);
        var resultFailure = Assert.IsType<Failure<EventTitle>>(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.TitleIsNull);
    }
    
    //UC2 - F5
    [Fact]
    public void Update_TitleOfEventWhenEventIsActive_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;
        var newTitle = EventTitle.Create("Hejjjjj").Value;
        evt.SetActiveStatus();
        

        // Act
        var newTitleResult = evt.UpdateTitle(newTitle);
        
        
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(newTitleResult);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidEventStatus);
    }
    
    //UC2 - F6
    [Fact]
    public void Update_TitleOfEventWhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var id = EventId.Create(Guid.NewGuid()).Value;
        var evt = Event.Create(id).Value;
        var newTitle = EventTitle.Create("Hejjjjj").Value;
        evt.SetCancelledStatus();
        

        // Act
        var newTitleResult = evt.UpdateTitle(newTitle);
        
        
        
        // Assert
        Assert.NotNull(newTitleResult);
        Assert.True(newTitleResult.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(newTitleResult);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTitle.InvalidEventStatus);
    }

    // S1: Draft event with valid description (0-250 chars)
    [Fact]
    public void UpdateDescription_WhenEventInDraftStatus_ShouldUpdateDescription()
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
    public void UpdateDescription_WhenSettingToEmpty_ShouldSetEmptyDescription()
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
    public void UpdateDescription_WhenEventInReadyStatus_ShouldUpdateAndChangeToDraft()
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
    public void UpdateDescription_WhenDescriptionTooLong_ShouldFailWithMessage()
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
    public void UpdateDescription_WhenEventIsCancelled_ShouldFailWithMessage()
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
    public void UpdateDescription_WhenEventIsActive_ShouldFailWithMessage()
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