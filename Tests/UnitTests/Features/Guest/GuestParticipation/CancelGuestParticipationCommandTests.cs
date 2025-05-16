using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestParticipation;

public class CancelGuestParticipationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = CancelGuestParticipationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Success<CancelGuestParticipationCommand>);
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
        var result = CancelGuestParticipationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Failure<CancelGuestParticipationCommand>);
    }
}
