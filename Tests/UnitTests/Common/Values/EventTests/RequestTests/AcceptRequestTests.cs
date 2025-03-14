using VIAEventAssociation.Core.Domain.Aggregates.Events.Request;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class AcceptRequestTests
{
    [Fact]
    public void AcceptRequest_WithValidIdAndPending_ReturnsSuccess()
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
        var result = request.AcceptRequest(requestId);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(RequestStatus.Accepted, request.requestStatus);
    }

    [Fact]
    public void AcceptRequest_WithInvalidId_ReturnsFailure()
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
        
        var differentIdResult = RequestId.Create(Guid.NewGuid());
        var differentId = differentIdResult is Success<RequestId> differentIdSuccess
            ? differentIdSuccess.Value
            : null;
        Assert.NotNull(differentId);
        
        // Act
        var result = request.AcceptRequest(differentId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.Mismatch);
    }

    [Fact]
    public void AcceptRequest_WhenAlreadyAccepted_ReturnsFailure()
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
        var result = request.AcceptRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotAccept);
    }

    [Fact]
    public void AcceptRequest_WhenAlreadyRejected_ReturnsFailure()
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
        var result = request.AcceptRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotAccept);
    }
}