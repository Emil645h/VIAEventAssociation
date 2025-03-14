using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public sealed record EventId
{
    public Guid Value { get; }
    
    private EventId(Guid value) => Value = value;

    public static Result<EventId> Create(Guid value) 
        => value == Guid.Empty ? EventErrors.EventId.IsEmpty : new EventId(value);
};