using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;

public static class InviteErrors
{
    public static class InviteId
    {
        private const string InviteIdCode = "Event.Invite.Id";

        public static ResultError IsEmpty = new(InviteIdCode, "Invite Id cannot be empty.");
        public static ResultError CannotAccept = new(InviteIdCode, "Cannot accept invite in its current state.");
        public static ResultError CannotReject = new(InviteIdCode, "Cannot reject invite in its current state.");
        public static ResultError Mismatch = new(InviteIdCode, "Mismatch invite ID.");
        
    }

    public static class AssignToGuestId
    {
        private const string AssignToGuestIdCode = "Event.Invite.AssignToGuestId";
        
        public static ResultError IsEmpty = new(AssignToGuestIdCode, "Guest Id cannot be empty.");
        public static ResultError AlreadyAssigned = new(AssignToGuestIdCode, "This invite is already assigned to a guest.");
    }
}