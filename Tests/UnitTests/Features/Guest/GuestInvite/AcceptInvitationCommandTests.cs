using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class AcceptInvitationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = AcceptInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Success<AcceptInvitationCommand>);
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
        var result = AcceptInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Failure<AcceptInvitationCommand>);
    }

    [Fact]
    public void Create_WithEmptyEventId_ReturnsFailure()
    {
        // Arrange
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = AcceptInvitationCommand.Create("", guestId);

        // Assert
        Assert.True(result is Failure<AcceptInvitationCommand>);
    }
}