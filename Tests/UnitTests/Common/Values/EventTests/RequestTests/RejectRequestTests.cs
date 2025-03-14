using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class RejectRequestTests
{
    [Fact]
    public void RejectRequest_WhenValidGuestIdAndPending_ReturnsSuccess()
    {
        // Arrange
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        
        // Act
        var result = request.RejectRequest(requestId);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(RequestStatus.Rejected, request.requestStatus);
    }

    [Fact]
    public void RejectRequest_WhenInvalidGuestId_ReturnsFailure()
    {
        // Arrange
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        var diffrentId = RequestId.Create(Guid.NewGuid()).Value;
        
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
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
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
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        request.AcceptRequest(requestId);
        
        // Act
        var result = request.RejectRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotReject);
    }
}