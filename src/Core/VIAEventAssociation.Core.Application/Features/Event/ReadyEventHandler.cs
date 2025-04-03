using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class ReadyEventHandler : ICommandHandler<ReadyEventCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;
    private readonly ICurrentTime currentTime;

    internal ReadyEventHandler(IEventRepository eventRepo, IUnitOfWork uow, ICurrentTime currentTime)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
        this.currentTime = currentTime;
    }

    public async Task<Result> HandleAsync(ReadyEventCommand command)
    {
        // Get the event
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        // Ready the event
        var readyResult = getResult.Value.SetReadyStatus(currentTime);
        if (readyResult.IsFailure)
            return readyResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}