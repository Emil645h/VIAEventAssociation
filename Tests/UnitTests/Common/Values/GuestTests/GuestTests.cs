using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.GuestTests;

public class GuestTests
{

    [Fact]
    public void Create_WithAllValidData_ReturnsGuest()
    {
        // Arrange
        var idResult = GuestId.Create(Guid.NewGuid());
        var emailResult = ViaEmail.Create("331458@via.dk");
        var firstNameResult = FirstName.Create("Emil");
        var lastNameResult = LastName.Create("Brügge");

        var guestId = ((Success<GuestId>)idResult).Value;
        var email = ((Success<ViaEmail>)emailResult).Value;
        var firstName = ((Success<FirstName>)firstNameResult).Value;
        var lastName = ((Success<LastName>)lastNameResult).Value;
        
        // Act
        var guestResult = Guest.Create(guestId, firstName, lastName, email);
        
        Assert.True(guestResult.IsSuccess);
        var success = Assert.IsType<Success<Guest>>(guestResult);
        var guest = success.Value;
        Assert.Equal(guestId, guest.Id);
        Assert.Equal(firstName, guest.firstName);
        Assert.Equal(lastName, guest.lastName);
        Assert.Equal(email, guest.email);
    }
}