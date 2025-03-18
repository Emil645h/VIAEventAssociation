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
}