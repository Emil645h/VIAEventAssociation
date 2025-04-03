using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class UpdateEventDescriptionHandler : ICommandHandler<UpdateEventDescriptionCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal UpdateEventDescriptionHandler(IEventRepository eventRepo, IUnitOfWork uow)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(UpdateEventDescriptionCommand command)
    {
        // Get the event
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        // Update description
        var updateResult = getResult.Value.UpdateDescription(command.Description);
        if (updateResult.IsFailure)
            return updateResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}