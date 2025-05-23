

using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;
using VIAEventAssociation.Infrastructure.SqliteDataRead.SeedFactories;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Extensions;

public static class DbContextSeedExtensions
{
    public static VeadatabaseProductionContext Seed(this VeadatabaseProductionContext context)
    {
        // Add guests
        context.Guests.AddRange(GuestSeedFactory.CreateGuests());
            
        // Add events
        List<Event> events = EventSeedFactory.CreateEvents();
        context.Events.AddRange(events);
        context.SaveChanges();
            
        // Create GuestLists for each event
        var guestLists = events.Select(e => new GuestList
        {
            Id = Guid.NewGuid().ToString(),
            EventId = e.Id
        }).ToList();

        context.GuestLists.AddRange(guestLists);
        context.SaveChanges();
        
        // Add participations
        ParticipationSeedFactory.Seed(context);
        context.SaveChanges();
            
        // Add invitations
        InviteSeedFactory.Seed(context);
        context.SaveChanges();
            
        return context;
    }
    
    public static VeadatabaseProductionContext SetupReadContext()
    {
        DbContextOptionsBuilder<VeadatabaseProductionContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() + ".db";
        optionsBuilder.UseSqlite("Data Source = " + testDbName);
            
        VeadatabaseProductionContext context = new(optionsBuilder.Options);
            
        // Ensure the database is fresh
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
            
        return context;
    }
}