using VIAEventAssociation.Core.Domain.Aggregates.Guests.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult.Errors;

namespace UnitTests.Common.Values.GuestTests;

public class ViaEmailTests
{
    [Theory]
    [InlineData("123456@via.dk")]
    [InlineData("123456@viauc.dk")]
    [InlineData("trmo@via.dk")]
    public void Create_WithValidEmail_ReturnsViaEmail(string email)
    {
        // Act
        var result = ViaEmail.Create(email);
        
        // Assert
        Assert.True(result.IsSuccess);
        
        var success = Assert.IsType<Success<ViaEmail>>(result);
        var viaEmail = success.Value;
        Assert.Equal(email, viaEmail.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_NullOrEmptyEmail_ReturnsFailure(string email)
    {
        var result = ViaEmail.Create(email);
        
        Assert.True(result.IsFailure);
        var failure = Assert.IsType<Failure<ViaEmail>>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ViaEmail.EmailIsEmpty);
    }

    [Theory]
    [InlineData("testdomain.com")]
    [InlineData("test@domain.co.uk")]
    [InlineData("name@notvia.dk")]
    public void Create_InvalidViaEmail_ReturnsFailure(string invalidEmail)
    {
        var result = ViaEmail.Create(invalidEmail);
        
        Assert.True(result.IsFailure);
        var failure = Assert.IsType<Failure<ViaEmail>>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ViaEmail.MustViaEmail);
    }

    [Theory]
    [InlineData("@some.dk")]
    [InlineData("someEmail.dk")]
    [InlineData("some@email")]
    public void Create_InvalidEmailStructure_ReturnsFailure(string invalidEmail)
    {
        var result = ViaEmail.Create(invalidEmail);
        
        Assert.True(result.IsFailure);
        var failure = Assert.IsType<Failure<ViaEmail>>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.ViaEmail.InvalidEmailStructure);
    }
}