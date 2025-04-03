using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Guest;

public class RegisterGuestHandler : ICommandHandler<RegisterGuestCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IUnitOfWork uow;

    internal RegisterGuestHandler(IGuestRepository guestRepo, IUnitOfWork uow)
    {
        this.guestRepo = guestRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(RegisterGuestCommand command)
    {
        // Check if email is already registered
        // var isEmailRegisteredResult = await guestRepo.IsEmailRegisteredAsync(command.Email);
        // if (isEmailRegisteredResult.IsFailure)
        //     return isEmailRegisteredResult;
        //
        // if (isEmailRegisteredResult.Value)
        //     return GuestErrors.ViaEmail.EmailAlreadyRegistered;

        // Create new guest ID
        var guestId = GuestId.Create(Guid.NewGuid()).Value;
        
        // Create the guest
        var createResult = VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.Guest.Create(
            guestId,
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.ProfilePictureUrl);
            
        if (createResult.IsFailure)
            return createResult;

        // Add to repository
        var addResult = await guestRepo.AddAsync(createResult.Value);
        if (addResult.IsFailure)
            return addResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}