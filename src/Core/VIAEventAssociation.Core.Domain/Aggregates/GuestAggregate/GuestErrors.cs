using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

public static class GuestErrors
{
    public static class GuestId
    {
        private const string GuestIdCode = "Guest.Id";

        public static ResultError IsEmpty = new(GuestIdCode, "Guest Id cannot be empty.");
    }
    
    public static class ViaEmail
    {
        private const string EmailCode = "Guest.ViaEmail";

        public static ResultError EmailIsEmpty = new(EmailCode, "Email cannot be empty.");
        public static ResultError MustViaEmail = new(EmailCode, "Email must be a valid Via Email.");
        public static ResultError InvalidEmailStructure = new(EmailCode, "Invalid Email structure.");
    }

    public static class FirstName
    {
        private const string FirstNameCode = "Guest.FirstName";
        
        public static ResultError FirstNameIsEmpty = new(FirstNameCode, "First Name cannot be empty.");
        public static ResultError InvalidCharacters = new(FirstNameCode, "Invalid Characters in First Name.");
    }

    public static class LastName
    {
        private const string LastNameCode = "Guest.LastName";
        
        public static ResultError LastNameIsEmpty = new(LastNameCode, "Last Name cannot be empty.");
        public static ResultError InvalidCharacters = new(LastNameCode, "Invalid Characters in Last Name.");
    }
}