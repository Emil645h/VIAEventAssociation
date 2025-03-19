using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

public static class GuestErrors
{
    public static class GuestId
    {
        private const string GuestIdCode = "Guest.Id";

        public static readonly ResultError IsEmpty = new(GuestIdCode, "Guest Id cannot be empty.");
    }
    
    public static class ViaEmail
    {
        private const string EmailCode = "Guest.ViaEmail";

        public static readonly ResultError EmailIsEmpty = new(EmailCode, "Email cannot be empty.");
        public static readonly ResultError MustViaEmail = new(EmailCode, "Email must be a valid Via Email.");
        public static readonly ResultError InvalidEmailStructure = new(EmailCode, "Email must be in the format <text1>@<text2>.<text3>.");
        public static readonly ResultError InvalidUsernameFormat = new(EmailCode, "Email username must be either 3-4 letters or 6 digits.");
        public static readonly ResultError EmailAlreadyRegistered = new(EmailCode, "This email is already registered.");
    }

    public static class FirstName
    {
        private const string FirstNameCode = "Guest.FirstName";
        
        public static readonly ResultError FirstNameIsEmpty = new(FirstNameCode, "First Name cannot be empty.");
        public static readonly ResultError InvalidCharacters = new(FirstNameCode, "First name can only contain letters (a-z).");
        public static readonly ResultError InvalidLength = new(FirstNameCode, "First name must be between 2 and 25 characters long.");
    }

    public static class LastName
    {
        private const string LastNameCode = "Guest.LastName";
        
        public static readonly ResultError LastNameIsEmpty = new(LastNameCode, "Last Name cannot be empty.");
        public static readonly ResultError InvalidCharacters = new(LastNameCode, "Last name can only contain letters (a-z).");
        public static readonly ResultError InvalidLength = new(LastNameCode, "Last name must be between 2 and 25 characters long.");
    }
    
    public static class ProfilePictureUrl
    {
        private const string ProfilePictureUrlCode = "Guest.ProfilePictureUrl";
        
        public static readonly ResultError UrlIsEmpty = new(ProfilePictureUrlCode, "Profile picture URL cannot be empty.");
        public static readonly ResultError InvalidUrlFormat = new(ProfilePictureUrlCode, "Profile picture URL has an invalid format.");
    }
}