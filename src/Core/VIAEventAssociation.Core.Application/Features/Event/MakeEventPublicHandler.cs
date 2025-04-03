using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class MakeEventPublicHandler : ICommandHandler<MakeEventPublicCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal MakeEventPublicHandler(IEventRepository eventRepo, IUnitOfWork uow)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(MakeEventPublicCommand command)
    {
        // Get the event
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        // Make event public
        var updateResult = getResult.Value.MakePublic();
        if (updateResult.IsFailure)
            return updateResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}