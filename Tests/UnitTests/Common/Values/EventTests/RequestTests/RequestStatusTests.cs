using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class RequestStatusTests
{
    [Fact]
    public void PendingRequest_CanAccept_ReturnsTrue()
    {
        // Arrange
        var status = RequestStatus.Pending;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.True(canAccept);
    }
    
    [Fact]
    public void PendingRequest_CanReject_ReturnsTrue()
    {
        // Arrange
        var status = RequestStatus.Pending;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.True(canReject);
    }
    
    [Fact]
    public void AcceptedRequest_CanAccept_ReturnsFalse()
    {
        // Arrange
        var status = RequestStatus.Accepted;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.False(canAccept);
    }
    
    [Fact]
    public void AcceptedRequest_CanReject_ReturnsFalse()
    {
        // Arrange
        var status = RequestStatus.Accepted;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.False(canReject);
    }
    
    [Fact]
    public void RejectedRequest_CanAccept_ReturnsFalse()
    {
        // Arrange
        var status = RequestStatus.Rejected;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.False(canAccept);
    }
    
    [Fact]
    public void RejectedRequest_CanReject_ReturnsFalse()
    {
        // Arrange
        var status = RequestStatus.Rejected;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.False(canReject);
    }
}