using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

internal class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;

    internal CreateEventHandler(IEventRepository eventRepo, IUnitOfWork uow)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
    }

    public async Task<Result> HandleAsync(CreateEventCommand command)
    {
        // Create
        var result = Domain.Aggregates.EventAggregate.Event.Create(command.Id);
        if (result.IsFailure)
            return result;

        // Add
        var addResult = await eventRepo.AddAsync(result.Value);
        if (addResult.IsFailure)
            return addResult;

        // Save
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
}