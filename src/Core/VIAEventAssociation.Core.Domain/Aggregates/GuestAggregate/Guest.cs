using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

public class Guest : AggregateRoot<GuestId>
{
    internal FirstName firstName;
    internal LastName lastName;
    internal ViaEmail email;
    
    private Guest(GuestId id, FirstName firstName, LastName lastName, ViaEmail email) : base(id) 
        => (this.firstName, this.lastName, this.email) = (firstName, lastName, email);
    
    public static Result<Guest> Create(GuestId id, FirstName firstName, LastName lastName, ViaEmail email) 
        => new Guest(id, firstName, lastName, email);

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
}