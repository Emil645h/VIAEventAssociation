using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;

public class RequestStatus : Enumeration
{
    public static readonly RequestStatus Pending = new RequestStatus(0, "Pending");
    public static readonly RequestStatus Accepted = new RequestStatus(1, "Accepted");
    public static readonly RequestStatus Rejected = new RequestStatus(2, "Rejected");
    
    private RequestStatus() { }
    private RequestStatus(int value, string displayName) : base(value, displayName) { }

    public bool CanAccept => this == Pending;
    public bool CanReject => this == Pending;
}