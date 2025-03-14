using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Invite;

public static class InviteErrors
{
    public static class InviteId
    {
        private const string InviteIdCode = "Event.Invite.Id";

        public static ResultError IsEmpty = new(InviteIdCode, "Invite Id cannot be empty.");
        public static ResultError AlreadyAccepted = new(InviteIdCode, "Cannot reject invite when invite is accepted.");
        public static ResultError AlreadyRejected = new(InviteIdCode, "Cannot accept invite when invite is rejected.");
    }

    public static class AssignToGuestId
    {
        private const string AssignToGuestIdCode = "Event.Invite.AssignToGuestId";
        
        public static ResultError IsEmpty = new(AssignToGuestIdCode, "Guest Id cannot be empty.");
        public static ResultError AlreadyAssigned = new(AssignToGuestIdCode, "This invite is already assigned to a guest.");
    }
}