using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class InviteStatusTests
{
    [Fact]
    public void ExtendedInvite_CanAccept_ReturnsTrue()
    {
        var status = InviteStatus.Extended;

        var canAccept = status.CanAccept;
        
        Assert.True(canAccept);
    }
    
    [Fact]
    public void ExtendedInvite_CanReject_ReturnsTrue()
    {
        var status = InviteStatus.Extended;

        var canReject = status.CanReject;
        
        Assert.True(canReject);
    }
    
    [Fact]
    public void AcceptedInvite_CanAccept_ReturnsFalse()
    {
        var status = InviteStatus.Accepted;

        var canAccept = status.CanAccept;
        
        Assert.False(canAccept);
    }
    
    [Fact]
    public void AcceptedInvite_CanReject_ReturnsFalse()
    {
        var status = InviteStatus.Accepted;

        var canReject = status.CanReject;
        
        Assert.False(canReject);
    }
    
    [Fact]
    public void RejectedInvite_CanAccept_ReturnsFalse()
    {
        var status = InviteStatus.Rejected;

        var canAccept = status.CanAccept;
        
        Assert.False(canAccept);
    }
    
    [Fact]
    public void RejectedInvite_CanReject_ReturnsFalse()
    {
        var status = InviteStatus.Rejected;

        var canReject = status.CanReject;
        
        Assert.False(canReject);
    }
}