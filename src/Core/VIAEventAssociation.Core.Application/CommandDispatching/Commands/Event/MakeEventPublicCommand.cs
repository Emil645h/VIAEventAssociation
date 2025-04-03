using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class MakeEventPublicCommand
{
    public EventId Id { get; }
    
    private MakeEventPublicCommand(EventId id)
        => this.Id = id;

    public static Result<MakeEventPublicCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultExtensions.CombineResultsInto<MakeEventPublicCommand>(idResult)
            .WithPayloadIfSuccess(() => new MakeEventPublicCommand(idResult.Value));
    }
}