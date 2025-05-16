using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Guest;

public class CancelGuestParticipationHandler : ICommandHandler<CancelGuestParticipationCommand>
{
    private readonly IGuestRepository guestRepo;
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;
    private readonly ICurrentTime currentTime;

    public CancelGuestParticipationHandler(
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

    public async Task<Result> HandleAsync(CancelGuestParticipationCommand command)
    {
        
        var guestResult = await guestRepo.GetByIdAsync(command.GuestId);
        if (guestResult is not Success<Domain.Aggregates.GuestAggregate.Guest> guestSuccess)
            return guestResult;

        var guest = guestSuccess.Value;

        
        var eventResult = await eventRepo.GetByIdAsync(command.EventId);
        if (eventResult is not Success<Domain.Aggregates.EventAggregate.Event> eventSuccess)
            return eventResult;

        var @event = eventSuccess.Value;

       
        var removeResult = @event.RemoveGuestFromGuestList(guest, currentTime);
        if (removeResult.IsFailure)
            return removeResult;

      
        await uow.SaveChangesAsync();

        return new Success<None>(new None());
    }
}
