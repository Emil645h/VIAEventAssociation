using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class UpdateEventMaxGuestsCommand
{
    public EventId Id { get; }
    public EventMaxGuests MaxGuests { get; }
    
    private UpdateEventMaxGuestsCommand(EventId id, EventMaxGuests maxGuests)
    {
        Id = id;
        MaxGuests = maxGuests;
    }

    public static Result<UpdateEventMaxGuestsCommand> Create(string id, int maxGuests)
    {
        var idResult = EventId.FromString(id);
        var maxGuestsResult = EventMaxGuests.Create(maxGuests);

        return ResultExtensions.CombineResultsInto<UpdateEventMaxGuestsCommand>(idResult, maxGuestsResult)
            .WithPayloadIfSuccess(() => new UpdateEventMaxGuestsCommand(idResult.Value, maxGuestsResult.Value));
    }
}