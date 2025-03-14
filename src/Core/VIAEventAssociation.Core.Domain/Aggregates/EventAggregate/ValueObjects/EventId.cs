using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public record EventId
{
    public Guid Value { get; }
    
    private EventId(Guid value) => Value = value;

    public static Result<EventId> Create(Guid eventId)
    {
        return new Success<EventId>(new EventId(eventId));
    }
};