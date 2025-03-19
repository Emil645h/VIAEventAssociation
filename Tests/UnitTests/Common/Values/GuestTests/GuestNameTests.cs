using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.GuestTests;

public class GuestNameTests
{
    [Theory]
    [InlineData("Emil")]
    [InlineData("Brugge")]
    public void Create_WithValidName_ReturnsFirstOrLastName(string name)
    {
        // Act
        var firstNameResult = FirstName.Create(name);
        var lastNameResult = LastName.Create(name);
        
        // Assert
        Assert.True(firstNameResult.IsSuccess);
        Assert.True(lastNameResult.IsSuccess);

        var firstNameSuccess = Assert.IsType<Success<FirstName>>(firstNameResult);
        var lastNameSuccess = Assert.IsType<Success<LastName>>(lastNameResult);
        var firstName = firstNameSuccess.Value;
        var lastName = lastNameSuccess.Value;
        Assert.Equal(name, firstName.Value);
        Assert.Equal(name, lastName.Value);
    }

    // UC 10, F3, F4 & F6
    [Theory]
    [InlineData("Emil1")] // Has a number
    [InlineData("Anne-Marie!")] // Has a special character
    [InlineData("Anne-Marie")] // Has "-"
    public void Create_WithInvalidName_ReturnsFailure(string name)
    {
        // Act
        var firstNameResult = FirstName.Create(name);
        var lastNameResult = LastName.Create(name);
        
        // Assert
        Assert.True(firstNameResult.IsFailure);
        Assert.True(lastNameResult.IsFailure);

        var firstNameFailure = Assert.IsType<Failure<FirstName>>(firstNameResult);
        var lastNameFailure = Assert.IsType<Failure<LastName>>(lastNameResult);
        Assert.Contains(firstNameFailure.Errors, e => e == GuestErrors.FirstName.InvalidCharacters);
        Assert.Contains(lastNameFailure.Errors, e => e == GuestErrors.LastName.InvalidCharacters);
    }
    
    // UC 10, F3 & F4
    [Theory]
    [InlineData("a")] // Too short
    [InlineData("abcdefghijklmnopqrstuvwxyz")] // Too long (26 characters)
    public void Create_WithInvalidNameLength_ReturnsFailure(string name)
    {
        // Act
        var firstNameResult = FirstName.Create(name);
        var lastNameResult = LastName.Create(name);
        
        // Assert
        Assert.True(firstNameResult.IsFailure);
        Assert.True(lastNameResult.IsFailure);

        var firstNameFailure = Assert.IsType<Failure<FirstName>>(firstNameResult);
        var lastNameFailure = Assert.IsType<Failure<LastName>>(lastNameResult);
        Assert.Contains(firstNameFailure.Errors, e => e == GuestErrors.FirstName.InvalidLength);
        Assert.Contains(lastNameFailure.Errors, e => e == GuestErrors.LastName.InvalidLength);
    }

    // UC 10, F3 & F4
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithNullOrEmptyName_ReturnsFailure(string name)
    {
        // Act
        var firstNameResult = FirstName.Create(name);
        var lastNameResult = LastName.Create(name);
        
        // Assert
        Assert.True(firstNameResult.IsFailure);
        Assert.True(lastNameResult.IsFailure);

        var firstNameFailure = Assert.IsType<Failure<FirstName>>(firstNameResult);
        var lastNameFailure = Assert.IsType<Failure<LastName>>(lastNameResult);
        Assert.Contains(firstNameFailure.Errors, e => e == GuestErrors.FirstName.FirstNameIsEmpty);
        Assert.Contains(lastNameFailure.Errors, e => e == GuestErrors.LastName.LastNameIsEmpty);
    }
}