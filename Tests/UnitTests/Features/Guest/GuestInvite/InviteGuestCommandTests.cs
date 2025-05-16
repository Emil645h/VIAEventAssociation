using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class InviteGuestCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = InviteGuestCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Success<InviteGuestCommand>);
        Assert.Equal(eventId, result.Value.EventId.Value.ToString());
        Assert.Equal(guestId, result.Value.GuestId.Value.ToString());
    }

    [Fact]
    public void Create_WithInvalidGuestId_ReturnsFailure()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var invalidGuestId = "not-a-guid";

        // Act
        var result = InviteGuestCommand.Create(eventId, invalidGuestId);

        // Assert
        Assert.True(result is Failure<InviteGuestCommand>);
    }

    [Fact]
    public void Create_WithEmptyEventId_ReturnsFailure()
    {
        // Arrange
        var emptyEventId = "";
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = InviteGuestCommand.Create(emptyEventId, guestId);

        // Assert
        Assert.True(result is Failure<InviteGuestCommand>);
    }
}