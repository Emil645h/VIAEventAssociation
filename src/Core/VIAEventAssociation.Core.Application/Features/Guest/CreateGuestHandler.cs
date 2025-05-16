using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Guest;


public class CreateGuestHandler : ICommandHandler<CreateGuestCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IUnitOfWork uow;

    internal CreateGuestHandler(IGuestRepository guestRepo, IUnitOfWork uow)
    {
        this.guestRepo = guestRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(CreateGuestCommand command)
    {
        var result = Domain.Aggregates.GuestAggregate.Guest.Create(
            command.Id,
            command.FirstName,
            command.LastName,
            command.Email,
            command.ProfilePictureUrl
        );
        if (result.IsFailure)
            return result;

        var addResult = await guestRepo.AddAsync(result.Value);
        if (addResult.IsFailure)
            return addResult;

        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}