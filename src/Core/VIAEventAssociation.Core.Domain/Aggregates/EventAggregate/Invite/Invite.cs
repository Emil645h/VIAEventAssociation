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
            return InviteErrors.AssignToGuestId.IsEmpty;

        var invite = new Invite(id, InviteStatus.Extended, guestId);
        invite.assignedGuestId = guestId;

        return invite;
    }


    public Result<None> AssignToInvite(GuestId guestId, EventStatus eventStatus, int currentGuestCount, int maxGuestCount, List<GuestId> invitedGuests, GuestList.GuestList guestList)
    {
        if (guestId == null)
            return InviteErrors.AssignToGuestId.IsEmpty;

        if (eventStatus != EventStatus.Ready && eventStatus != EventStatus.Active)
            return InviteErrors.AssignToEventId.EventNotActive;
    
        if (currentGuestCount >= maxGuestCount)
            return InviteErrors.AssignToEventId.EventIsFull;
    
        if (invitedGuests.Contains(guestId))
            return InviteErrors.AssignToEventId.GuestIsAlreadyInvited;

        if (guestList.IsGuestParticipating(guestId))
            return InviteErrors.AssignToEventId.GuestIsAlreadyParticipating;

        if (assignedGuestId != guestId)
            return InviteErrors.InviteId.Mismatch;

        return new None();
    }
    
    

    public Result<None> AcceptInvite(InviteId inviteId, GuestId guestId, Event evt)
    {
        if (inviteId == null)
            return InviteErrors.InviteId.IsEmpty;
    
        if (Id != inviteId)
            return InviteErrors.InviteId.Mismatch;

        if (!inviteStatus.CanAccept)
            return InviteErrors.InviteId.CannotAccept;
        
        if (assignedGuestId != guestId)
            return InviteErrors.InviteId.Mismatch;
        
        if (evt.guestList.numberOfGuests >= evt.maxGuests.Value)
            return InviteErrors.AssignToEventId.EventIsFull;
        
        if (evt.status != EventStatus.Active)
            return InviteErrors.AssignToEventId.EventNotActive;
        
        if (evt.eventTime.EndTime < DateTime.Now)
            return InviteErrors.AssignToEventId.EventIsPast;
        
    
        inviteStatus = InviteStatus.Accepted;
    
        return new None();
    }

    public Result<None> AcceptInvitePast(InviteId inviteId, GuestId guestId, Event evt, ICurrentTime currentTime)
    {
        if (evt.eventTime.EndTime < currentTime.GetCurrentTime())
            return InviteErrors.AssignToEventId.EventIsPast;

        inviteStatus = InviteStatus.Accepted;
        return new None();
    }


    public Result<None> RejectInvite(InviteId inviteId, GuestId guestId, Event evt)
    {
        if (Id != inviteId)
            return InviteErrors.InviteId.IsEmpty;
        
        if (assignedGuestId != guestId)
            return InviteErrors.InviteId.Mismatch;
        
        if (!inviteStatus.CanReject && inviteStatus != InviteStatus.Accepted)
            return InviteErrors.InviteId.CannotReject;

        
        if (!evt.status.Equals(EventStatus.Active))
            return InviteErrors.AssignToEventId.EventNotActive;

        if (inviteStatus.Equals(InviteStatus.Accepted))
        {
            inviteStatus = InviteStatus.Rejected;
            return new None();
        }
        
        inviteStatus = InviteStatus.Rejected;
        return new None();
    }
}