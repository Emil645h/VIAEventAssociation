using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.RegisterGuest;

public class RegisterGuestCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccessResult()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, validFirstName, validLastName, validProfilePictureUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(validEmail.ToLower(), result.Value.Email.Value);
        Assert.Equal("John", result.Value.FirstName.Value);
        Assert.Equal("Doe", result.Value.LastName.Value);
        Assert.Equal(validProfilePictureUrl, result.Value.ProfilePictureUrl.Value.ToString());
    }

    [Fact]
    public void Create_WithInvalidEmail_ReturnsError()
    {
        // Arrange
        var invalidEmail = "john@example.com"; // Not a VIA email
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(invalidEmail, validFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.MustViaEmail);
    }
    
    [Fact]
    public void Create_WithEmptyEmail_ReturnsError()
    {
        // Arrange
        var emptyEmail = "";
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(emptyEmail, validFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.EmailIsEmpty);
    }

    [Fact]
    public void Create_WithInvalidEmailStructure_ReturnsError()
    {
        // Arrange
        var invalidEmail = "abcdvia.dk"; // Missing @ symbol
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(invalidEmail, validFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.InvalidEmailStructure);
    }

    [Fact]
    public void Create_WithInvalidUsernameFormat_ReturnsError()
    {
        // Arrange
        var invalidEmail = "ab1@via.dk"; // Contains both letters and numbers
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(invalidEmail, validFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.InvalidUsernameFormat);
    }

    [Fact]
    public void Create_WithFirstNameTooShort_ReturnsError()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var invalidFirstName = "J"; // Too short
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, invalidFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.FirstName.InvalidLength);
    }

    [Fact]
    public void Create_WithFirstNameTooLong_ReturnsError()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var invalidFirstName = new string('J', 26); // Too long (26 characters)
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, invalidFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.FirstName.InvalidLength);
    }

    [Fact]
    public void Create_WithInvalidFirstNameCharacters_ReturnsError()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var invalidFirstName = "John123"; // Contains numbers
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, invalidFirstName, validLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.FirstName.InvalidCharacters);
    }

    [Fact]
    public void Create_WithInvalidLastNameCharacters_ReturnsError()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var validFirstName = "John";
        var invalidLastName = "Doe123"; // Contains numbers
        var validProfilePictureUrl = "https://example.com/picture.jpg";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, validFirstName, invalidLastName, validProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.LastName.InvalidCharacters);
    }

    [Fact]
    public void Create_WithInvalidProfilePictureUrl_ReturnsError()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var validFirstName = "John";
        var validLastName = "Doe";
        var invalidProfilePictureUrl = "not-a-url";

        // Act
        var result = RegisterGuestCommand.Create(validEmail, validFirstName, validLastName, invalidProfilePictureUrl);
        var resultFailure = Assert.IsType<Failure<RegisterGuestCommand>>(result);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ProfilePictureUrl.InvalidUrlFormat);
    }
}