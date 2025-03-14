using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.GuestListTests;

public class GuestListTests
{
    [Fact]
    public void Create_WithAllValidData_ReturnsGuestList()
    {
        // Arrange
        var guestListId = GuestListId.Create(Guid.NewGuid()).Value;
        
        // Act
        var guestList = GuestList.Create(guestListId);
        var guestListResult = ((Success<GuestList>) guestList).Value;
        
        // Assert
        Assert.True(guestList.IsSuccess);
        Assert.Equal(guestListId, guestListResult.Id);
    }
}