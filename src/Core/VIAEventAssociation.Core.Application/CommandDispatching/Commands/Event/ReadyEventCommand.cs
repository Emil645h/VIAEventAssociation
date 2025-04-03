using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class ReadyEventCommand
{
    public EventId Id { get; }
    
    private ReadyEventCommand(EventId id)
    {
        Id = id;
    }

    public static Result<ReadyEventCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultExtensions.CombineResultsInto<ReadyEventCommand>(idResult)
            .WithPayloadIfSuccess(() => new ReadyEventCommand(idResult.Value));
    }
}