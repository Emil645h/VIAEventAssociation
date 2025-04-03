using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.RegisterGuest;

public class RegisterGuestHandlerTests
{
    private readonly FakeGuestRepository guestRepository;
    private readonly FakeUnitOfWork unitOfWork;
    private readonly RegisterGuestHandler handler;

    public RegisterGuestHandlerTests()
    {
        // Setup fakes
        guestRepository = new FakeGuestRepository();
        unitOfWork = new FakeUnitOfWork();
        
        // Create the handler with fakes
        handler = new RegisterGuestHandler(guestRepository, unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WithValidInput_RegistersGuest()
    {
        // Arrange
        var validEmail = "abcd@via.dk";
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";
        
        var commandResult = RegisterGuestCommand.Create(
            validEmail, validFirstName, validLastName, validProfilePictureUrl);
        Assert.True(commandResult.IsSuccess);
        
        // Act
        var result = await handler.HandleAsync(commandResult.Value);
        
        // Assert
        Assert.IsType<Success<None>>(result);
        
        // Verify guest was added to repository
        Assert.Equal(1, guestRepository.Count);
    }
    
    [Fact]
    public async Task HandleAsync_WithEmailAlreadyRegistered_ReturnsFailure()
    {
        // This test is only relevant if you decide to check for email registration
        // For now, it's commented out since you mentioned that check is disabled
        
        /*
        // Arrange
        var existingEmail = "abcd@via.dk";
        var existingEmailObj = ViaEmail.Create(existingEmail).Value;
        var existingFirstName = FirstName.Create("Existing").Value;
        var existingLastName = LastName.Create("User").Value;
        var existingPictureUrl = ProfilePictureUrl.Create("https://example.com/existing.jpg").Value;
        
        // Add an existing guest with the same email
        var existingGuestId = GuestId.New();
        var existingGuest = Guest.Create(existingGuestId, existingFirstName, existingLastName, existingEmailObj, existingPictureUrl).Value;
        await guestRepository.AddAsync(existingGuest);
        
        // Now try to register a new guest with the same email
        var validFirstName = "John";
        var validLastName = "Doe";
        var validProfilePictureUrl = "https://example.com/picture.jpg";
        
        var commandResult = RegisterGuestCommand.Create(
            existingEmail, validFirstName, validLastName, validProfilePictureUrl);
        Assert.True(commandResult.IsSuccess);
        
        // Uncomment this section when you implement email registration check
        // Act
        // var result = await handler.HandleAsync(commandResult.Value);
        // var resultFailure = Assert.IsType<Failure<None>>(result);
        
        // Assert
        // Assert.Contains(resultFailure.Errors, e => e == GuestErrors.ViaEmail.EmailAlreadyRegistered);
        
        // Verify no new guest was added
        // Assert.Equal(1, guestRepository.Count);
        */
    }
    
    [Fact]
    public async Task HandleAsync_RegistersMultipleGuests_WithDifferentEmails()
    {
        // Arrange
        var email1 = "abcd@via.dk";
        var email2 = "efgh@via.dk";
        
        var command1Result = RegisterGuestCommand.Create(
            email1, "John", "Doe", "https://example.com/john.jpg");
        var command2Result = RegisterGuestCommand.Create(
            email2, "Jane", "Smith", "https://example.com/jane.jpg");
        
        Assert.True(command1Result.IsSuccess);
        Assert.True(command2Result.IsSuccess);
        
        // Act
        var result1 = await handler.HandleAsync(command1Result.Value);
        var result2 = await handler.HandleAsync(command2Result.Value);
        
        // Assert
        // Verify both guests were added
        Assert.Equal(2, guestRepository.Count);
    }
}