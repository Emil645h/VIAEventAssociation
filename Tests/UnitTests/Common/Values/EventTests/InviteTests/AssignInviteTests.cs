using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class AssignInviteTests
{
    [Fact]
    public void AssignInviteTo_WithValidGuestId_ReturnsSuccess()
    {
        // Arrange
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        Assert.True(inviteIdResult.IsSuccess);

        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        Assert.True(inviteResult.IsSuccess);
        
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);
        
        var guestIdResult = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult.IsSuccess);
        
        var guestId = guestIdResult is Success<GuestId> guestSuccess
            ? guestSuccess.Value
            : null;
        Assert.NotNull(guestId);

        // Act
        var result = invite.AssignToInvite(guestId);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AssignInviteTo_WhenAlreadyAssigned_ReturnsFailure()
    {
        // Arrange
        var inviteIdResult = InviteId.Create(Guid.NewGuid());
        Assert.True(inviteIdResult.IsSuccess);

        var inviteId = inviteIdResult is Success<InviteId> success
            ? success.Value
            : null;
        Assert.NotNull(inviteId);
        
        var inviteResult = Invite.Create(inviteId);
        Assert.True(inviteResult.IsSuccess);
        
        var invite = inviteResult is Success<Invite> inviteSuccess
            ? inviteSuccess.Value
            : null;
        Assert.NotNull(invite);
        
        var guestIdResult = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult.IsSuccess);
        
        var guestId = guestIdResult is Success<GuestId> guestSuccess
            ? guestSuccess.Value
            : null;
        Assert.NotNull(guestId);
        
        var guestIdResult2 = GuestId.Create(Guid.NewGuid());
        Assert.True(guestIdResult2.IsSuccess);
        
        var guestId2 = guestIdResult is Success<GuestId> guestSuccess2
            ? guestSuccess2.Value
            : null;
        Assert.NotNull(guestId2);
        
        // Act
        invite.AssignToInvite(guestId);
        var result = invite.AssignToInvite(guestId2);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToGuestId.AlreadyAssigned);
    }
}