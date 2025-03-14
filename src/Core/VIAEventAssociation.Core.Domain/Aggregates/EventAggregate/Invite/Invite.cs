using VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;

public class Invite : Entity<InviteId>
{
    internal InviteStatus inviteStatus;
    private GuestId? assignedGuestId;
    
    private Invite(InviteId id, InviteStatus inviteStatus) : base(id) 
        => this.inviteStatus = inviteStatus;
    
    public static Result<Invite> Create(InviteId id) 
        => new Invite(id, InviteStatus.Extended);

    public Result<None> AssignToInvite(GuestId guestId)
    {
        if (guestId == null)
            return InviteErrors.AssignToGuestId.IsEmpty;

        if (assignedGuestId != null)
            return InviteErrors.AssignToGuestId.AlreadyAssigned;
        
        assignedGuestId = guestId;
        return new None();
    }

    public Result<None> AcceptInvite(InviteId inviteId)
    {
        if (inviteId == null)
            return InviteErrors.InviteId.IsEmpty;
        
        if (Id != inviteId)
            return InviteErrors.InviteId.Mismatch;

        if (!inviteStatus.CanAccept)
            return InviteErrors.InviteId.CannotAccept;
        
        inviteStatus = InviteStatus.Accepted;
        return new None();
    }

    public Result<None> RejectInvite(InviteId inviteId)
    {
        if (inviteId == null)
            return InviteErrors.InviteId.IsEmpty;
        
        if (Id != inviteId)
            return InviteErrors.InviteId.Mismatch;
        
        if (!inviteStatus.CanReject)
            return InviteErrors.InviteId.CannotReject;
        
        inviteStatus = InviteStatus.Rejected;
        return new None();
    }
}