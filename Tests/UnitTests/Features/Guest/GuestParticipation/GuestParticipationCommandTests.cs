using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestParticipation;

public class GuestParticipationCommandTests
{
    
    [Fact]
    public void CreateCommand_WithValidGuids_ReturnsSuccess()
    {
        // Arrange
        var eventId = Guid.NewGuid().ToString();
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = GuestParticipationCommand.Create(eventId, guestId);

        // Assert
        Assert.True(result is Success<GuestParticipationCommand>);
        Assert.Equal(eventId, result.Value.EventId.Value.ToString());
        Assert.Equal(guestId, result.Value.GuestId.Value.ToString());
    }
    
    [Fact]
    public void CreateCommand_WithInvalidEventId_ReturnsFailure()
    {
        // Arrange
        var invalidEventId = "not-a-guid";
        var guestId = Guid.NewGuid().ToString();

        // Act
        var result = GuestParticipationCommand.Create(invalidEventId, guestId);

        // Assert
        Assert.True(result is Failure<GuestParticipationCommand>);
    }


}