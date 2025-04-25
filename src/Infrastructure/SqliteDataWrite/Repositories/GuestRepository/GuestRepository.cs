using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace SqliteDataWrite.Repositories.GuestRepository;

public class GuestRepository (WriteDbContext context) : RepositoryEfcBase<Guest, GuestId> (context), IGuestRepository
{
    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        var guests = await context.Guests.ToListAsync();
        return guests;
    }

    public async Task<Guest> GetByEmailAsync(ViaEmail email)
    {
        var guests = await context.Guests.ToListAsync();
        
        var guest = guests.FirstOrDefault(g => g.email.Value == email.Value);
        
        if (guest == null)
            throw new KeyNotFoundException($"Guest with email {email.Value} was not found.");

        return guest;
    }
}