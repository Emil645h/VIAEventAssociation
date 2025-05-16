using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Invite;

namespace VIAEventAssociation.Core.Application.Features.Invite;

using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public class DeclineInvitationHandler : ICommandHandler<DeclineInvitationCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal DeclineInvitationHandler(
        IGuestRepository guestRepo,
        IEventRepository eventRepo,
        IUnitOfWork uow)
    {
        this.guestRepo = guestRepo;
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(DeclineInvitationCommand command)
    {

        var guestResult = await guestRepo.GetByIdAsync(command.GuestId);
        if (guestResult is not Success<Guest> guestSuccess)
            return guestResult;

        var guest = guestSuccess.Value;


        var eventResult = await eventRepo.GetByIdAsync(command.EventId);
        if (eventResult is not Success<Event> eventSuccess)
            return eventResult;

        var @event = eventSuccess.Value;

        // 3. Decline invitation
        var declineResult = @event.GuestRejectsInvite(guest);
        if (declineResult.IsFailure)
            return declineResult;

        // 4. Gem ændringer
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}
