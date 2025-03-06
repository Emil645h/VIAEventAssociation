namespace VIAEventAssociation.Core.Tools.OperationResult.OperationResult.Errors;

public static class GuestErrors
{
    public static class ViaEmail
    {
        private const string EmailCode = "Guest.ViaEmail";

        public static ResultError EmailIsEmpty = new(EmailCode, "Email cannot be empty.");
        public static ResultError MustViaEmail = new(EmailCode, "Email must be a valid Via Email.");
        public static ResultError InvalidEmailStructure = new(EmailCode, "Invalid Email structure.");
    }
}