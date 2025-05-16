using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Guest;

public class InviteGuestHandler : ICommandHandler<InviteGuestCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal InviteGuestHandler(
        IGuestRepository guestRepo,
        IEventRepository eventRepo,
        IUnitOfWork uow)
    {
        this.guestRepo = guestRepo;
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(InviteGuestCommand command)
    {
        
        var guestResult = await guestRepo.GetByIdAsync(command.GuestId);
        if (guestResult is not Success<Domain.Aggregates.GuestAggregate.Guest> guestSuccess)
            return guestResult;

        var guest = guestSuccess.Value;

        
        var eventResult = await eventRepo.GetByIdAsync(command.EventId);
        if (eventResult is not Success<Domain.Aggregates.EventAggregate.Event> eventSuccess)
            return eventResult;

        var @event = eventSuccess.Value;

        
        var inviteResult = @event.CreateGuestInvite(guest);
        if (inviteResult.IsFailure)
            return inviteResult;

        
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}