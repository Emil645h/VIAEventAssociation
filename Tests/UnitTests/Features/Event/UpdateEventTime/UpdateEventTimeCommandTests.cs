using UnitTests.Fakes;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.UpdateEventTime;

public class UpdateEventTimeCommandTests
{
    private readonly StubCurrentTime currentTimeStub;

    public UpdateEventTimeCommandTests()
    {
        currentTimeStub = new StubCurrentTime(new DateTime(2023, 8, 20, 12, 0, 0));
    }

    [Fact]
    public void Create_WithValidTimeSameDay_ReturnsSuccessResult()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var startTime = new DateTime(2023, 8, 25, 13, 0, 0); // 1 PM
        var endTime = new DateTime(2023, 8, 25, 16, 0, 0);   // 4 PM (3 hours duration)

        // Act
        var result = UpdateEventTimeCommand.Create(validId, startTime, endTime, currentTimeStub);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validId, result.Value.Id.Value.ToString());
        Assert.Equal(startTime, result.Value.EventTime.StartTime);
        Assert.Equal(endTime, result.Value.EventTime.EndTime);
    }

    [Fact]
    public void Create_WithStartDateAfterEndDate_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var startTime = new DateTime(2023, 8, 26, 13, 0, 0);
        var endTime = new DateTime(2023, 8, 25, 16, 0, 0); // Day before start

        // Act
        var result = UpdateEventTimeCommand.Create(validId, startTime, endTime, currentTimeStub);
        var resultFailure = Assert.IsType<Failure<UpdateEventTimeCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartDateAfterEndDate);
    }

    [Fact]
    public void Create_WithStartTimeInPast_ReturnsError()
    {
        // Arrange
        var validId = Guid.NewGuid().ToString();
        var startTime = currentTimeStub.TheTime.AddHours(-1); // 1 hour before current time
        var endTime = currentTimeStub.TheTime.AddHours(3);    // 3 hours after current time

        // Act
        var result = UpdateEventTimeCommand.Create(validId, startTime, endTime, currentTimeStub);
        var resultFailure = Assert.IsType<Failure<UpdateEventTimeCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == EventErrors.EventTime.StartTimeInPast);
    }
}