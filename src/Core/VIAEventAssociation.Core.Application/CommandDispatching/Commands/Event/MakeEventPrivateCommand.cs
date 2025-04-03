using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class MakeEventPrivateCommand
{
    public EventId Id { get; }
    
    private MakeEventPrivateCommand(EventId id)
        => this.Id = id;

    public static Result<MakeEventPrivateCommand> Create(string id)
    {
        var idResult = EventId.FromString(id);

        return ResultExtensions.CombineResultsInto<MakeEventPrivateCommand>(idResult)
            .WithPayloadIfSuccess(() => new MakeEventPrivateCommand(idResult.Value));
    }
}