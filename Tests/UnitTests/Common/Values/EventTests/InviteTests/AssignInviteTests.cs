using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.InviteTests;

public class AssignInviteTests
{
    [Fact]
    public void AssignInviteTo_WithValidGuestId_ReturnsSuccess()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        var guestId = GuestId.Create(Guid.NewGuid()).Value;

        // Act
        var result = invite.AssignToInvite(guestId);
        
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AssignInviteTo_WhenAlreadyAssigned_ReturnsFailure()
    {
        // Arrange
        var inviteId = InviteId.Create(Guid.NewGuid()).Value;
        var invite = Invite.Create(inviteId).Value;
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        var guestId2 = GuestId.Create(Guid.NewGuid()).Value;

        
        // Act
        invite.AssignToInvite(guestId);
        var result = invite.AssignToInvite(guestId2);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == InviteErrors.AssignToGuestId.AlreadyAssigned);
    }
}