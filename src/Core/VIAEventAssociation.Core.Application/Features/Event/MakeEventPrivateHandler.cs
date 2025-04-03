using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class MakeEventPrivateHandler : ICommandHandler<MakeEventPrivateCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal MakeEventPrivateHandler(IEventRepository eventRepo, IUnitOfWork uow)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(MakeEventPrivateCommand command)
    {
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        var updateResult = getResult.Value.MakePrivate();
        if (updateResult.IsFailure)
            return updateResult;

        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}