using VIAEventAssociation.Core.Domain.Aggregates.Events.Request;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class AssignRequestTests
{
    [Fact]
    public void AssignRequest_WithValidGuestIdAndReason_ReturnsSuccess()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        Assert.True(requestIdResult.IsSuccess);

        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        Assert.True(requestResult.IsSuccess);
        
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(request);
        
        var guestIdResult = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult.IsSuccess);
        
        var guestId = guestIdResult is Success<GuestId> guestSuccess
            ? guestSuccess.Value
            : null;
        Assert.NotNull(guestId);

        // Act
        var result = request.AssignToRequest(guestId);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AssignRequest_WhenAlreadyAssigned_ReturnsFailure()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        Assert.True(requestIdResult.IsSuccess);

        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        Assert.True(requestResult.IsSuccess);
        
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(request);
        
        var guestIdResult = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult.IsSuccess);
        
        var guestId = guestIdResult is Success<GuestId> guestSuccess
            ? guestSuccess.Value
            : null;
        Assert.NotNull(guestId);
        
        var guestIdResult2 = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult2.IsSuccess);
        
        var guestId2 = guestIdResult2 is Success<GuestId> guestIdResult2Success
            ? guestIdResult2Success.Value
            : null;
        Assert.NotNull(guestId2);
        
        // Act
        request.AssignToRequest(guestId);
        var result = request.AssignToRequest(guestId2);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.AssignToGuestId.AlreadyAssigned);
    }
}