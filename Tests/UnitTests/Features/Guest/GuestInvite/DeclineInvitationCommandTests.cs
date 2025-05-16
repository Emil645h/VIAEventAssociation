using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Invite;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class DeclineInvitationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = DeclineInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Success<DeclineInvitationCommand>);
        Assert.Equal(eventId, result.Value.EventId.Value.ToString());
        Assert.Equal(guestId, result.Value.GuestId.Value.ToString());
    }

    [Fact]
    public void Create_WithInvalidGuestId_ReturnsFailure()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = "not-a-guid";

        // Act
        var result = DeclineInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Failure<DeclineInvitationCommand>);
    }

    [Fact]
    public void Create_WithEmptyEventId_ReturnsFailure()
    {
        // Arrange
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = DeclineInvitationCommand.Create("", guestId);

        // Assert
        Assert.True(result is Failure<DeclineInvitationCommand>);
    }
}