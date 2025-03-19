using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.GuestTests;

public class ProfilePictureUrlTests
{
    [Theory]
    [InlineData("http://example.com/profile.jpg")]
    [InlineData("https://example.com/profile.jpg")]
    [InlineData("http://example.com/profile")]
    [InlineData("https://example.com/profile")]
    public void Create_WithAllValidData_ReturnsSuccess(string profilePictureUrl)
    {
        // Act
        var result = ProfilePictureUrl.Create(profilePictureUrl);
        var uriResult = result.Value.Value.AbsoluteUri;
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(profilePictureUrl, uriResult);
    }
    
    // UC 10, F7
    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/profile.jpg")] // Not http/https
    public void Create_InvalidProfilePictureUrl_ReturnsFailure(string profilePictureUrl)
    {
        // Act
        var result = ProfilePictureUrl.Create(profilePictureUrl);
        var resultFailure = Assert.IsType<Failure<ProfilePictureUrl>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ProfilePictureUrl.InvalidUrlFormat);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Create_NullOrEmptyProfilePictureUrl_ReturnsFailure(string profilePictureUrl)
    {
        // Act
        var result = ProfilePictureUrl.Create(profilePictureUrl);
        var resultFailure = Assert.IsType<Failure<ProfilePictureUrl>>(result);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ProfilePictureUrl.UrlIsEmpty);
    }
}