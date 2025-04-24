using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request.ValueObjects;

public sealed record RequestId
{
    public Guid Value { get; }
    
    private RequestId(Guid value) => Value = value;
    
    public static Result<RequestId> Create(Guid value) =>
    value == Guid.Empty ? RequestErrors.RequestId.IsEmpty : new RequestId(value);
    
    public static Result<RequestId> FromGuid(Guid guid) 
        => guid == Guid.Empty ? RequestErrors.RequestId.IsEmpty : new RequestId(guid);

}