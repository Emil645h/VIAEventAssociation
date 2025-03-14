using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Request;

public static class RequestErrors
{
    public static class RequestId
    {
        private const string RequestIdCode = "Event.Request.Id";
        
        public static ResultError IsEmpty = new(RequestIdCode, "Request Id cannot be empty.");
        public static ResultError CannotAccept = new(RequestIdCode, "Cannot accept request in its current state.");
        public static ResultError CannotReject = new(RequestIdCode, "Cannot reject request in its current state.");
        public static ResultError Mismatch = new(RequestIdCode, "Mismatch reject ID.");
    }

    public static class Reason
    {
        private const string RequestReasonCode = "Event.Request.Reason";
        
        public static ResultError IsEmpty = new(RequestReasonCode, "Request reason cannot be empty.");
    }

    public static class AssignToGuestId
    {
        private const string AssignToGuestIdCode = "Event.Request.AssignToGuestId";
        
        public static ResultError IsEmpty = new(AssignToGuestIdCode, "Guest Id cannot be empty.");
        public static ResultError AlreadyAssigned = new(AssignToGuestIdCode, "This request is already assigned to a guest.");
    }
}