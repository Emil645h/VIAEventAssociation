using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;

namespace UnitTests.Features.Guest.CreateGuest;

public class CreateGuestCommandTests
{
    [Fact]
    public void Create_WithValidId_ReturnsCommand()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var firstName = "John";
        var lastName = "Doe";
        var email = "123456@via.dk";
        var profilePicUrl =
            "https://media.istockphoto.com/id/521573873/vector/unknown-person-silhouette-whith-blue-tie.jpg?s=2048x2048&w=is&k=20&c=cjOrS4d7gV46uXDx9iWH5n5uSEF6hhZ6Gebbp5j6USI=";
        
        // Act
        var result = CreateGuestCommand.Create(id, firstName, lastName, email, profilePicUrl);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Id);
        Assert.Equal(Guid.Parse(id), result.Value.Id.Value);
    }
    
    [Fact]
    public void Create_WithInvalidEmail_ReturnsCommand()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var firstName = "John";
        var lastName = "Doe";
        var email = "johndoe@hotmail.com";
        var profilePicUrl =
            "https://media.istockphoto.com/id/521573873/vector/unknown-person-silhouette-whith-blue-tie.jpg?s=2048x2048&w=is&k=20&c=cjOrS4d7gV46uXDx9iWH5n5uSEF6hhZ6Gebbp5j6USI=";
        
        // Act
        var result = CreateGuestCommand.Create(id, firstName, lastName, email, profilePicUrl);
        
        // Assert
        Assert.True(result.IsFailure);
    }
}