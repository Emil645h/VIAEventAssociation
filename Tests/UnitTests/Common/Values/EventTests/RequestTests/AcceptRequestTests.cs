using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class AcceptRequestTests
{
    [Fact]
    public void AcceptRequest_WithValidIdAndPending_ReturnsSuccess()
    {
        // Arrange
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        
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
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        var differentId = RequestId.Create(Guid.NewGuid()).Value;
        
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
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;
        
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
        var requestId = RequestId.Create(Guid.NewGuid()).Value;
        string reason = "I'm super passionate about VIA.";
        var request = Request.Create(requestId, reason).Value;

        request.RejectRequest(requestId);
        
        // Act
        var result = request.AcceptRequest(requestId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.RequestId.CannotAccept);
    }
}