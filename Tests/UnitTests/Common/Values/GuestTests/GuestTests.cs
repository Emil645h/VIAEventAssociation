using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.GuestTests;

public class GuestTests : IDisposable
{

    private readonly GuestId guestId;
    private readonly FirstName firstName;
    private readonly LastName lastName;
    private readonly ViaEmail viaEmail;
    private readonly Guest guest;
    private readonly ProfilePictureUrl profilePictureUrl;
    
    // TODO: Add mocking for a future service that checks if email is already registered in DB.
    
    public GuestTests()
    {
        guestId = GuestId.Create(Guid.NewGuid()).Value;
        viaEmail = ViaEmail.Create("331458@via.dk").Value;
        firstName = FirstName.Create("Emil").Value;
        lastName = LastName.Create("Brugge").Value;
        profilePictureUrl = ProfilePictureUrl.Create("https://via.dk/someImage").Value;
        guest = Guest.Create(guestId, firstName, lastName, viaEmail, profilePictureUrl).Value;
    }

    public void Dispose()
    {
        // Nothing
    }
    
    // UC 10, S1
    [Fact]
    public void Create_WithAllValidData_ReturnsGuest()
    {
        
        // Act
        var guestResult = Guest.Create(guestId, firstName, lastName, viaEmail, profilePictureUrl);
        
        // Assert
        Assert.True(guestResult.IsSuccess);
        var success = Assert.IsType<Success<Guest>>(guestResult);
        var guest = success.Value;
        Assert.Equal(guestId, guest.Id);
        Assert.Equal(firstName, guest.firstName);
        Assert.Equal(lastName, guest.lastName);
        Assert.Equal(viaEmail, guest.email);
        Assert.Equal(profilePictureUrl, guest.profilePictureUrl);
    }

    // UC 10, F5
    [Fact]
    public async Task Create_EmailAlreadyRegistered_ReturnsFailure()
    {
        // TODO: Implement when DB attached.
    }
    
    [Theory]
    [InlineData("john", "smith", "abcd@via.dk", "http://example.com/profile.jpg", "John", "Smith", "abcd@via.dk")]
    [InlineData("JOHN", "SMITH", "ABCD@via.dk", "http://example.com/profile.jpg", "John", "Smith", "abcd@via.dk")]
    [InlineData("jOhN", "sMiTh", "AbCd@VIA.DK", "http://example.com/profile.jpg", "John", "Smith", "abcd@via.dk")]
    public async Task Create_FormatsCasing_Correctly(
        string inputFirstName, string inputLastName, string inputEmail, string profilePictureUrl,
        string expectedFirstName, string expectedLastName, string expectedEmail)
    {
        // Arrange
        var firstName = FirstName.Create(inputFirstName).Value;
        var lastName = LastName.Create(inputLastName).Value;
        var email = ViaEmail.Create(inputEmail).Value;
        var profilePicUrl = ProfilePictureUrl.Create(profilePictureUrl).Value;
        
        // Act
        var result = Guest.Create(guestId, firstName, lastName, email, profilePicUrl);
        
        // Assert
        Assert.True(result.IsSuccess);
        var guest = result.Value;
        Assert.Equal(expectedFirstName, guest.firstName.Value);
        Assert.Equal(expectedLastName, guest.lastName.Value);
        Assert.Equal(expectedEmail, guest.email.Value);
    }
    
    #region UpdateViaEmail Tests

    [Fact]
    public void UpdateViaEmail_WithValidEmail_ReturnsSuccess()
    {
        // Arrange
        var newEmail = ViaEmail.Create("abcd@via.dk");
        var email = ((Success<ViaEmail>)newEmail).Value;
        
        // Act
        var result = guest.UpdateViaEmail(email);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, guest.email);
    }

    [Fact]
    public void UpdateViaEmail_WithNullOrEmptyEmail_ReturnsFailure()
    {
        // Act
        var result = guest.UpdateViaEmail(null);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.EmailIsEmpty);
    }

    [Fact]
    public void UpdateViaEmail_ChangesEmailValue_ReturnsSuccess()
    {
        // Arrange
        var orignalEmail = guest.email;
        var newEmail = ViaEmail.Create("abcd@via.dk");
        var email = ((Success<ViaEmail>)newEmail).Value;
        
        // Act
        guest.UpdateViaEmail(email);
        
        // Assert
        Assert.NotEqual(orignalEmail, guest.email);
        Assert.Equal(newEmail, guest.email);
        Assert.Equal("abcd@via.dk", guest.email.Value);
    }

    #endregion
    
    #region UpdateFirstName Tests
    
    [Fact]
    public void UpdateFirstName_WithValidName_ReturnsSuccess()
    {
        // Arrange
        var newNameResult = FirstName.Create("Jane");
        var newName = ((Success<FirstName>)newNameResult).Value;
        
        // Act
        var result = guest.UpdateFirstName(newName);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, guest.firstName);
    }
    
    [Fact]
    public void UpdateFirstName_WithNullName_ReturnsFailure()
    {
        // Act
        var result = guest.UpdateFirstName(null);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.FirstName.FirstNameIsEmpty);
    }
    
    [Fact]
    public void UpdateFirstName_ChangesNameValue()
    {
        // Arrange
        var originalName = guest.firstName;
        var newNameResult = FirstName.Create("Richard");
        var newName = ((Success<FirstName>)newNameResult).Value;
        
        // Act
        guest.UpdateFirstName(newName);
        
        // Assert
        Assert.NotEqual(originalName, guest.firstName);
        Assert.Equal(newName, guest.firstName);
        Assert.Equal("Richard", guest.firstName.Value);
    }
    
    #endregion
    
    #region UpdateLastName Tests
    
    [Fact]
    public void UpdateLastName_WithValidName_ReturnsSuccess()
    {
        // Arrange
        var newNameResult = LastName.Create("Smith");
        var newName = ((Success<LastName>)newNameResult).Value;
        
        // Act
        var result = guest.UpdateLastName(newName);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, guest.lastName);
    }
    
    [Fact]
    public void UpdateLastName_WithNullName_ReturnsFailure()
    {
        // Act
        var result = guest.UpdateLastName(null);
        
        // Assert
        Assert.True(result.IsFailure);
        var resultFailure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(resultFailure.Errors, e => e == GuestErrors.LastName.LastNameIsEmpty);
    }
    
    [Fact]
    public void UpdateLastName_ChangesNameValue()
    {
        // Arrange
        var originalName = guest.lastName;
        var newNameResult = LastName.Create("Johnson");
        var newName = ((Success<LastName>)newNameResult).Value;
        
        // Act
        guest.UpdateLastName(newName);
        
        // Assert
        Assert.NotEqual(originalName, guest.lastName);
        Assert.Equal(newName, guest.lastName);
        Assert.Equal("Johnson", guest.lastName.Value);
    }
    
    #endregion
}