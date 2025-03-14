using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class AcceptInviteTests
{
    [Fact]
    public void AcceptInvite_WithValidIdAndExtended_ReturnsSuccess()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        
        // Act
        var result = invite.AcceptInvite(inviteId);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InviteStatus.Accepted, invite.inviteStatus);
    }

    [Fact]
    public void AcceptInvite_WithInvalidId_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        var differentId = InviteId.Create(Guid.NewGuid()).Value;
        
        // Act
        var result = invite.AcceptInvite(differentId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.Mismatch);
    }

    [Fact]
    public void AcceptInvite_WhenAlreadyAccepted_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        invite.AcceptInvite(inviteId);
        
        // Act
        var result = invite.AcceptInvite(inviteId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.CannotAccept);
    }

    [Fact]
    public void AcceptInvite_WhenAlreadyRejected_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        
        var invite = Invite.Create(inviteId).Value;
        invite.RejectInvite(inviteId);
        
        // Act
        var result = invite.AcceptInvite(inviteId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.CannotAccept);
    }
}