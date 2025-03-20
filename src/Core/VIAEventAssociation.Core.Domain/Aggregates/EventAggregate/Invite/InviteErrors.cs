using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;

public static class InviteErrors
{
    public static class InviteId
    {
        private const string InviteIdCode = "Event.Invite.Id";

        public static readonly ResultError IsEmpty = new(InviteIdCode, "Invite Id cannot be empty.");
        public static readonly ResultError CannotAccept = new(InviteIdCode, "Cannot accept invite in its current state.");
        public static readonly ResultError CannotReject = new(InviteIdCode, "Cannot reject invite in its current state.");
        public static readonly ResultError Mismatch = new(InviteIdCode, "Mismatch invite ID.");
        
    }

    public static class AssignToGuestId
    {
        private const string AssignToGuestIdCode = "Event.Invite.AssignToGuestId";
        
        public static readonly ResultError IsEmpty = new(AssignToGuestIdCode, "Guest Id cannot be empty.");
        public static readonly ResultError AlreadyAssigned = new(AssignToGuestIdCode, "This invite is already assigned to a guest.");
    }
    
    public static class AssignToEventId
    {
        private const string AssignToEventIdCode = "Event.Invite.AssignToEventId";
        
        public static readonly ResultError EventNotActive = new(AssignToEventIdCode, "Event is not ready or active.");
        public static readonly ResultError EventIsFull = new(AssignToEventIdCode, "Event is full.");
        public static readonly ResultError GuestIsAlreadyInvited = new(AssignToEventIdCode, "Guest is already invited.");
        public static readonly ResultError GuestIsAlreadyParticipating = new(AssignToEventIdCode, "Guest is already participating.");
        public static readonly ResultError EventIsPast = new(AssignToEventIdCode, "Event is in the past.");

    }
}