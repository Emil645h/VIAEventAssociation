using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.GuestListTests;

public class GuestListTests
{
    private readonly GuestListId guestListId;
    private readonly GuestList guestList;
    private readonly GuestId guestId;
    
    public GuestListTests()
    {
        guestListId = GuestListId.Create(Guid.NewGuid()).Value;
        guestList = GuestList.Create(guestListId).Value;
        guestId = GuestId.Create(Guid.NewGuid()).Value;
    }
    
    [Fact]
    public void Create_WithValidId_ReturnsSuccessResult()
    {
        // Arrange
        var id = GuestListId.Create(Guid.NewGuid()).Value;
        
        // Act
        var result = GuestList.Create(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result is Success<GuestList> success ? success.Value : null);
    }
    
    [Fact]
    public void Create_NewGuestList_HasZeroGuests()
    {
        // Assert
        Assert.Equal(0, guestList.numberOfGuests);
    }
    
    [Fact]
    public void AssignToGuestList_WithValidGuestId_ReturnsSuccess()
    {
        // Act
        var result = guestList.AssignToGuestList(guestId);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, guestList.numberOfGuests);
    }
    
    [Fact]
    public void AssignToGuestList_SameGuestTwice_ReturnsFailure()
    {
        // Arrange
        guestList.AssignToGuestList(guestId);
        
        // Act
        var result = guestList.AssignToGuestList(guestId);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Equal(1, guestList.numberOfGuests);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.AssignToGuestList.GuestAlreadyAssigned);
    }
    
    [Fact]
    public void AssignToGuestList_NullGuestId_ReturnsFailure()
    {
        // Act
        var result = guestList.AssignToGuestList(null);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Equal(0, guestList.numberOfGuests);
        Assert.Contains(resultFailure.Errors, e => e == GuestListErrors.AssignToGuestList.GuestIsEmpty);
    }
}