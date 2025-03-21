using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;

public static class InviteErrors
{
    public static class InviteId
    {
        private const string InviteIdCode = "Event.Invite.Id";

        public static readonly ResultError IsEmpty = new(InviteIdCode, "Invite Id cannot be empty.");
    }

    public static class AcceptInvite
    {
        private const string AcceptInviteCode = "Event.Invite.Accept";
        
        public static readonly ResultError AlreadyAccepted = new(AcceptInviteCode, "Invite has already been accepted.");
        public static readonly ResultError InvitationRejected = new(AcceptInviteCode, "This invitation has been rejected.");
    }
    
    public static class RejectInvite
    {
        private const string RejectInviteCode = "Event.Invite.Reject";
        
        public static readonly ResultError AlreadyRejected = new(RejectInviteCode, "Invite has already been rejected.");
    }
    
    public static class Invite
    {
        private const string InviteCode = "Event.Invite";

        public static readonly ResultError GuestIdIsEmpty = new(InviteCode, "Guest Id cannot be null or empty.");
        public static readonly ResultError EventNotReadyOrActive = new(InviteCode, "Event is not ready or active.");
        public static readonly ResultError EventIsFull = new(InviteCode, "Event is full.");
        public static readonly ResultError GuestIsAlreadyInvited = new(InviteCode, "Guest is already invited.");
        public static readonly ResultError GuestIsAlreadyParticipating = new(InviteCode, "Guest is already participating.");
        public static readonly ResultError EventIsPast = new(InviteCode, "Event is in the past.");
    }
}