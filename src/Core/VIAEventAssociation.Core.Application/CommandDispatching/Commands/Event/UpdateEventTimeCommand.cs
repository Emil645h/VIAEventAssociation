using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;

public class UpdateEventTimeCommand
{
    public EventId Id { get; }
    public EventTime EventTime { get; }
    
    private UpdateEventTimeCommand(EventId id, EventTime eventTime)
    {
        Id = id;
        EventTime = eventTime;
    }

    public static Result<UpdateEventTimeCommand> Create(string id, DateTime startTime, DateTime endTime, ICurrentTime currentTime)
    {
        var idResult = EventId.FromString(id);
        var eventTimeResult = EventTime.Create(startTime, endTime, currentTime);

        return ResultExtensions.CombineResultsInto<UpdateEventTimeCommand>(idResult, eventTimeResult)
            .WithPayloadIfSuccess(() => new UpdateEventTimeCommand(idResult.Value, eventTimeResult.Value));
    }
}