﻿using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class AcceptInviteTests
{
    [Fact]
    public void AcceptInvite_WithValidIdAndExtended_ReturnsSuccess()
    {
        // Arrange
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);

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
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);
        
        var differentIdResult = InviteId.Create(Guid.NewGuid());
        var differentId = differentIdResult is Success<InviteId> differentSuccess
            ? differentSuccess.Value
            : null;
        Assert.NotNull(differentId);
        
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
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);
        
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
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);
        invite.RejectInvite(inviteId);
        
        // Act
        var result = invite.AcceptInvite(inviteId);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.InviteId.CannotAccept);
    }
}