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
    
    public GuestTests()
    {
        var idResult = GuestId.Create(Guid.NewGuid());
        var emailResult = ViaEmail.Create("331458@via.dk");
        var firstNameResult = FirstName.Create("Emil");
        var lastNameResult = LastName.Create("Brügge");
        guestId = ((Success<GuestId>)idResult).Value;
        viaEmail = ((Success<ViaEmail>)emailResult).Value;
        firstName = ((Success<FirstName>)firstNameResult).Value;
        lastName = ((Success<LastName>)lastNameResult).Value;
        var guestResult = Guest.Create(guestId, firstName, lastName, viaEmail);
        
        guest = ((Success<Guest>)guestResult).Value;
    }

    public void Dispose()
    {
        // Nothing
    }
    
    [Fact]
    public void Create_WithAllValidData_ReturnsGuest()
    {
        
        // Act
        var guestResult = Guest.Create(guestId, firstName, lastName, viaEmail);
        
        // Assert
        Assert.True(guestResult.IsSuccess);
        var success = Assert.IsType<Success<Guest>>(guestResult);
        var guest = success.Value;
        Assert.Equal(guestId, guest.Id);
        Assert.Equal(firstName, guest.firstName);
        Assert.Equal(lastName, guest.lastName);
        Assert.Equal(viaEmail, guest.email);
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