using VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.LocationTests;

public class LocationNameTests
{
    [Fact]
    public void Can_Create_LocationName()
    {
        // Arrange
        string input = "Test Location";

        // Act
        Result<LocationName> created = LocationName.Create(input);
        
        // Assert
        Assert.True(created.IsSuccess);
        
        var success = Assert.IsType<Success<LocationName>>(created);
        var locationName = success.Value;
        Assert.Equal(input, locationName.Value);
    }
    
    
}