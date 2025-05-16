using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.CreateGuest;

public class CreateGuestHandlerTests
{
    private GuestId _guestId;
    private CreateGuestCommand _cmd;
    private FakeGuestRepository _guestRepo;
    private FakeUnitOfWork _uow;
    private ICommandHandler<CreateGuestCommand> _handler;

    private void SetupBasic()
    {
        var id = Guid.NewGuid().ToString();
        var firstName = "John";
        var lastName = "Doe";
        var email = "123456@via.dk";
        var profilePicture = "https://example.com/profile.jpg";
        _guestId = GuestId.FromString(id).Value;
        _cmd = CreateGuestCommand.Create(id, firstName, lastName, email, profilePicture).Value;
        
        _guestRepo = new FakeGuestRepository();
        _uow = new FakeUnitOfWork();
        
        _handler = new CreateGuestHandler(_guestRepo, _uow);
    }

    [Fact]
    public async Task Handle_ValidInput_Success()
    {
        // Arrange
        SetupBasic();
        
        // Act
        var result = await _handler.HandleAsync(_cmd);
        
        // Assert
        Assert.IsAssignableFrom<Success<None>>(result);
        
        // Verify guest was added to repository with correct properties
        var retrieveResult = await _guestRepo.GetByIdAsync(_guestId);
        var createdGuest = retrieveResult.Value;
        
        Assert.Equal("John", createdGuest.firstName.Value);
        Assert.Equal("Doe", createdGuest.lastName.Value);
        Assert.Equal("123456@via.dk", createdGuest.email.Value);
    }
}