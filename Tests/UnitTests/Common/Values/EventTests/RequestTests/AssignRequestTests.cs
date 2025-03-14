using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class AssignRequestTests
{
    [Fact]
    public void AssignRequest_WithValidGuestIdAndReason_ReturnsSuccess()
    {
        // Arrange
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        var guestId = GuestId.Create(Guid.NewGuid()).Value;

        // Act
        var result = request.AssignToRequest(guestId);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AssignRequest_WhenAlreadyAssigned_ReturnsFailure()
    {
        // Arrange
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var guestId2 = GuestId.Create(Guid.NewGuid()).Value;
        
        // Act
        request.AssignToRequest(guestId);
        var result = request.AssignToRequest(guestId2);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.AssignToGuestId.AlreadyAssigned);
    }
}