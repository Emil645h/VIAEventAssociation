using VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.LocationTests;

public class LocationIdTests
{
    [Fact]
    public void Can_Create_LocationId()
    {
        // Arrange
        Guid input = Guid.NewGuid();

        // Act
        Result<LocationId> created = LocationId.Create(input);
        
        // Assert
        Assert.True(created.IsSuccess);
        
        var success = Assert.IsType<Success<LocationId>>(created);
        var locationId = success.Value;
        Assert.Equal(input, locationId.Value);
    }
}