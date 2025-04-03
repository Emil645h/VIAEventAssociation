using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class UpdateEventDescriptionCommand
{
    public EventId Id { get; }
    public EventDescription Description { get; }
    
    private UpdateEventDescriptionCommand(EventId id, EventDescription description)
        => (Id, Description) = (id, description);
    
    public static Result<UpdateEventDescriptionCommand> Create(string id, string description)
    {
        var idResult = EventId.FromString(id);
        var descriptionResult = EventDescription.Create(description);

        return ResultExtensions.CombineResultsInto<UpdateEventDescriptionCommand>(idResult, descriptionResult)
            .WithPayloadIfSuccess(() => new UpdateEventDescriptionCommand(idResult.Value, descriptionResult.Value));
    }
}