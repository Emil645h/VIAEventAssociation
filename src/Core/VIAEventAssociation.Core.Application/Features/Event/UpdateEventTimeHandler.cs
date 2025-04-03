using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class UpdateEventTimeHandler : ICommandHandler<UpdateEventTimeCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal UpdateEventTimeHandler(IEventRepository eventRepo, IUnitOfWork uow)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(UpdateEventTimeCommand command)
    {
        // Get the event
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        // Update time
        var updateResult = getResult.Value.UpdateTime(command.EventTime);
        if (updateResult.IsFailure)
            return updateResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}