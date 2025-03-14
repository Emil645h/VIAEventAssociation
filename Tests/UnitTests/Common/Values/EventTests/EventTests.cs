using VIAEventAssociation.Core.Domain.Aggregates.Events;
using VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests;

public class EventTests
{
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
    }
}