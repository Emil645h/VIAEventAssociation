using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class ActivateEventCommand
{
    public EventId Id { get; }
    
    private ActivateEventCommand(EventId id)
    {
        Id = id;
    }

    public static Result<ActivateEventCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultExtensions.CombineResultsInto<ActivateEventCommand>(idResult)
            .WithPayloadIfSuccess(() => new ActivateEventCommand(idResult.Value));
    }
}