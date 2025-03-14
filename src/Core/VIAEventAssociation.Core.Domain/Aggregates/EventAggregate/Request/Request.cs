using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Request;

public class Request : Entity<RequestId>
{
    internal RequestStatus requestStatus;
    internal string reason;
    private GuestId? assignedGuestId;
    
    private Request(RequestId id, RequestStatus requestStatus, string reason) : base(id)
        => (this.requestStatus, this.reason) = (requestStatus, reason);
    
    public static Result<Request> Create(RequestId id, string reason) 
        => string.IsNullOrWhiteSpace(reason) ? RequestErrors.Reason.IsEmpty : new Request(id, RequestStatus.Pending, reason);

    public Result<None> AssignToRequest(GuestId guestId)
    {
        if (guestId == null)
            return RequestErrors.AssignToGuestId.IsEmpty;

        if (assignedGuestId != null)
            return RequestErrors.AssignToGuestId.AlreadyAssigned;
        
        assignedGuestId = guestId;
        return new None();
    }

    public Result<None> AcceptRequest(RequestId requestId)
    {
        if (requestId == null)
            return RequestErrors.RequestId.IsEmpty;

        if (Id != requestId)
            return RequestErrors.RequestId.Mismatch;

        if (!requestStatus.CanAccept) 
            return RequestErrors.RequestId.CannotAccept;
        
        requestStatus = RequestStatus.Accepted;
        return new None();
    }

    public Result<None> RejectRequest(RequestId requestId)
    {
        if (requestId == null)
            return RequestErrors.RequestId.IsEmpty;
        
        if (Id != requestId)
            return RequestErrors.RequestId.Mismatch;
        
        if (!requestStatus.CanReject)
            return RequestErrors.RequestId.CannotReject;
        
        requestStatus = RequestStatus.Rejected;
        return new None();
    }
}