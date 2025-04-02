using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class CreateEventCommand
{
    public EventId Id { get; }
    
    private CreateEventCommand(EventId id) => Id = id;

    public static Result<CreateEventCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return Result<None>.Combine(idResult.Map(_ => new None()))
            .WithPayloadIfSuccess(() => new CreateEventCommand(idResult.Value));
    }
}