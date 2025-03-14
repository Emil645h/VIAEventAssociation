using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class InviteAssignmentTests
{
    [Fact]
    public void AssignInviteTo_WithValidGuestId_ReturnsSuccess()
    {
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

        var result = invite.AssignToInvite(guestId);
        
        Assert.True(result.IsSuccess);
    }
}