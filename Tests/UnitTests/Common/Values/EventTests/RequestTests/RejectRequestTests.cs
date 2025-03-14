using VIAEventAssociation.Core.Domain.Aggregates.Events.Request;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class RejectRequestTests
{
    [Fact]
    public void RejectRequest_WhenValidGuestIdAndPending_ReturnsSuccess()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(requestResult);
        
        // Act
        var result = request.RejectRequest(requestId);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(RequestStatus.Rejected, request.requestStatus);
    }

    [Fact]
    public void RejectRequest_WhenInvalidGuestId_ReturnsFailure()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(requestResult);
        
        var diffrentIdResult = RequestId.Create(Guid.NewGuid());
        var diffrentId = diffrentIdResult is Success<RequestId> diffrentIdSuccess
            ? diffrentIdSuccess.Value
            : null;
        Assert.NotNull(diffrentId);
        
        // Act
        var result = request.RejectRequest(diffrentId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.Mismatch);
    }

    [Fact]
    public void RejectRequest_WhenAlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(requestResult);
        request.RejectRequest(requestId);
        
        // Act
        var result = request.RejectRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotReject);
    }

    [Fact]
    public void RejectRequest_WhenAlreadyAccepted_ReturnsFailure()
    {
        // Arrange
        var requestIdResult = RequestId.Create(Guid.NewGuid());
        var requestId = requestIdResult is Success<RequestId> success
            ? success.Value
            : null;
        Assert.NotNull(requestId);

        string reason = "I'm super passionate about VIA.";
        
        var requestResult = Request.Create(requestId, reason);
        var request = requestResult is Success<Request> requestSuccess
            ? requestSuccess.Value
            : null;
        Assert.NotNull(requestResult);
        request.AcceptRequest(requestId);
        
        // Act
        var result = request.RejectRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotReject);
    }
}