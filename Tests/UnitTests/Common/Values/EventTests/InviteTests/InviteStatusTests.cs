using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class InviteStatusTests
{
    [Fact]
    public void ExtendedInvite_CanAccept_ReturnsTrue()
    {
        // Arrange
        var status = InviteStatus.Extended;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.True(canAccept);
    }
    
    [Fact]
    public void ExtendedInvite_CanReject_ReturnsTrue()
    {
        // Arrange
        var status = InviteStatus.Extended;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.True(canReject);
    }
    
    [Fact]
    public void AcceptedInvite_CanAccept_ReturnsFalse()
    {
        // Arrange
        var status = InviteStatus.Accepted;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.False(canAccept);
    }
    
    [Fact]
    public void AcceptedInvite_CanReject_ReturnsFalse()
    {
        // Arrange
        var status = InviteStatus.Accepted;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.False(canReject);
    }
    
    [Fact]
    public void RejectedInvite_CanAccept_ReturnsFalse()
    {
        // Arrange
        var status = InviteStatus.Rejected;

        // Act
        var canAccept = status.CanAccept;
        
        // Assert
        Assert.False(canAccept);
    }
    
    [Fact]
    public void RejectedInvite_CanReject_ReturnsFalse()
    {
        // Arrange
        var status = InviteStatus.Rejected;

        // Act
        var canReject = status.CanReject;
        
        // Assert
        Assert.False(canReject);
    }
}