using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

public class Guest : AggregateRoot<GuestId>
{
    internal FirstName firstName;
    internal LastName lastName;
    internal ViaEmail email;
    internal ProfilePictureUrl profilePictureUrl;
    
    private Guest(GuestId id, FirstName firstName, LastName lastName, ViaEmail email, ProfilePictureUrl profilePictureUrl) : base(id) 
        => (this.firstName, this.lastName, this.email, this.profilePictureUrl) = (firstName, lastName, email, profilePictureUrl);
    
    private Guest() { } // EFC
    
    public static Result<Guest> Create(GuestId id, FirstName firstName, LastName lastName, ViaEmail email, ProfilePictureUrl profilePictureUrl) 
        => new Guest(id, firstName, lastName, email, profilePictureUrl);
    
    public Result<None> UpdateViaEmail(ViaEmail viaEmail)
    {
        if (viaEmail == null)
            return GuestErrors.ViaEmail.EmailIsEmpty;
        
        email = viaEmail;
        return new None();
    }

    public Result<None> UpdateFirstName(FirstName newName)
    {
        if (newName == null)
            return GuestErrors.FirstName.FirstNameIsEmpty;
        
        firstName = newName;
        return new None();
    }
    public Result<None> UpdateLastName(LastName newName)
    {
        if (newName == null)
            return GuestErrors.LastName.LastNameIsEmpty;
        
        lastName = newName;
        return new None();
    }

    public Result<None> UpdateProfilePictureUrl(ProfilePictureUrl newUrl)
    {
        if (newUrl == null)
            return GuestErrors.ProfilePictureUrl.UrlIsEmpty;
        
        profilePictureUrl = newUrl;
        return new None();
    }
}