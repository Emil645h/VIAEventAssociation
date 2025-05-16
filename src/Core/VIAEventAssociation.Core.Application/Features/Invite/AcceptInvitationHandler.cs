using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;

namespace VIAEventAssociation.Core.Application.Features.Guest;

using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public class AcceptInvitationHandler : ICommandHandler<AcceptInvitationCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;
    private readonly ICurrentTime currentTime;

    internal AcceptInvitationHandler(
        IGuestRepository guestRepo,
        IEventRepository eventRepo,
        IUnitOfWork uow,
        ICurrentTime currentTime)
    {
        this.guestRepo = guestRepo;
        this.eventRepo = eventRepo;
        this.uow = uow;
        this.currentTime = currentTime;
    }

    public async Task<Result> HandleAsync(AcceptInvitationCommand command)
    {
       
        var guestResult = await guestRepo.GetByIdAsync(command.GuestId);
        if (guestResult is not Success<Guest> guestSuccess)
            return guestResult;

        var guest = guestSuccess.Value;

        
        var eventResult = await eventRepo.GetByIdAsync(command.EventId);
        if (eventResult is not Success<Event> eventSuccess)
            return eventResult;

        var @event = eventSuccess.Value;

       
        var acceptResult = @event.GuestAcceptsInvite(guest, currentTime);
        if (acceptResult.IsFailure)
            return acceptResult;

        
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}
