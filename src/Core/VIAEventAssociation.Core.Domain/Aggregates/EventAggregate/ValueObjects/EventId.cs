using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public sealed record EventId
{
    public Guid Value { get; }
    
    private EventId(Guid value) => Value = value;

    public static Result<EventId> Create(Guid value) 
        => value == Guid.Empty ? EventErrors.EventId.IsEmpty : new EventId(value);

    public static Result<EventId> FromString(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return EventErrors.EventId.IsEmpty;

        if (!Guid.TryParse(id, out var guidResult))
            return EventErrors.EventId.IsEmpty;
        
        return new EventId(guidResult);
    }
    
    public static Result<EventId> FromGuid(Guid guid) 
        => guid == Guid.Empty ? EventErrors.EventId.IsEmpty : new EventId(guid);

}