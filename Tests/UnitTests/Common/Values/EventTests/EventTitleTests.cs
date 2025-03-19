using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventTitleTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2025, 1, 1, 12, 0, 0));

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
        eventResult.Value.SetReadyStatus(_defaultTime);
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
        evt.SetActiveStatus(_defaultTime);


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
}