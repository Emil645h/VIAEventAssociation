using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;

public class Invite : Entity<InviteId>
{
    internal InviteStatus inviteStatus;
    internal GuestId assignedGuestId;

    private Invite(InviteId id, InviteStatus inviteStatus, GuestId assignedGuestId) : base(id)
    {
        (this.inviteStatus, this.assignedGuestId) = (inviteStatus, assignedGuestId);
    }
    
    public static Result<Invite> Create(InviteId id, GuestId guestId) 
    {
        if (guestId == null)
            return InviteErrors.Invite.GuestIdIsEmpty;

        var invite = new Invite(id, InviteStatus.Extended, guestId);
        invite.assignedGuestId = guestId;

        return invite;
    }

    public Result<None> AcceptInvite()
    {
        if (inviteStatus.Equals(InviteStatus.Accepted))
            return InviteErrors.AcceptInvite.AlreadyAccepted;

        if (inviteStatus.Equals(InviteStatus.Rejected))
            return InviteErrors.AcceptInvite.InvitationRejected;
        
        inviteStatus = InviteStatus.Accepted;
        return new None();
    }

    public Result<None> RejectInvite()
    {
        if (inviteStatus.Equals(InviteStatus.Rejected))
            return InviteErrors.RejectInvite.AlreadyRejected;
        
        inviteStatus = InviteStatus.Rejected;
        return new None();
    }
}