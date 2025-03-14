using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class RejectInviteTests
{
    [Fact]
    public void RejectInvite_WhenValidIdAndExtended_ReturnsSuccess()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        
        // Act
        var result = invite.RejectInvite(inviteId);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(InviteStatus.Rejected, invite.inviteStatus);
    }

    [Fact]
    public void RejectInvite_WhenInvalidId_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        var differentId = InviteId.Create(Guid.NewGuid()).Value;
        
        // Act
        var result = invite.RejectInvite(differentId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.Mismatch);
    }

    [Fact]
    public void RejectInvite_WhenAlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        
        var invite = Invite.Create(inviteId).Value;
        invite.RejectInvite(inviteId);
        
        // Act
        var result = invite.RejectInvite(inviteId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.CannotReject);
    }

    [Fact]
    public void RejectInvite_WhenAlreadyAccepted_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        invite.AcceptInvite(inviteId);
        
        // Act
        var result = invite.RejectInvite(inviteId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.CannotReject);
    }
}