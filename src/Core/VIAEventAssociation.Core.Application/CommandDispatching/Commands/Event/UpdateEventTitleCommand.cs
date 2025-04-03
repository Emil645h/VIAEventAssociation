using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class UpdateEventTitleCommand
{
    public EventId Id { get; }
    public EventTitle Title { get; }
    
    private UpdateEventTitleCommand(EventId id, EventTitle title) 
        => (Id, Title) = (id, title);

    public static Result<UpdateEventTitleCommand> Create(string id, string title)
    {
        Result<EventId> idResult = EventId.FromString(id);
        Result<EventTitle> titleResult = EventTitle.Create(title);

        return ResultExtensions.CombineResultsInto<UpdateEventTitleCommand>(idResult, titleResult)
            .WithPayloadIfSuccess(() => new UpdateEventTitleCommand(idResult.Value, titleResult.Value));
    }
}