using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories.EventRepository;

public class EventRepository (WriteDbContext context) : RepositoryEfcBase<Event, EventId>(context), IEventRepository
{
    protected override async Task<Event> FindByIdAsync(EventId id)
    {
        return await context.Events
            .Include(e => e.guestList)
            .ThenInclude(gl => EF.Property<HashSet<GuestId>>(gl, "guests"))
            .Include(e => e.invites)
            .Include(e => e.requests)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        var events = await context.Events
            .Include(e => e.guestList)
            .Include(e => e.invites)
            .Include(e => e.requests)
            .ToListAsync();
            
        return events;
    }
}