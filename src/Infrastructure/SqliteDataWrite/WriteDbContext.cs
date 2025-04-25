using Microsoft.EntityFrameworkCore;
using SqliteDataWrite.Configurations;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

namespace SqliteDataWrite;

public class WriteDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Guest> Guests => Set<Guest>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RequestConfigurations());
        modelBuilder.ApplyConfiguration(new InviteConfigurations());
        modelBuilder.ApplyConfiguration(new EventConfigurations());
        modelBuilder.ApplyConfiguration(new GuestConfigurations());
    }
    
    /// <summary>
    /// Helper method for testing.
    /// Creates a DbContext and sets up a fresh database.
    /// </summary>
    /// <returns></returns>
    public static WriteDbContext SetupContext()
    {
        DbContextOptionsBuilder<WriteDbContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() +".db";
        optionsBuilder.UseSqlite(@"Data Source = " + testDbName);
        WriteDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
    
    /// <summary>
    /// Helper method for testing.
    /// Saves an entity and clears the ChangeTracker (the cache).
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    private static async Task SaveAndClearAsync<T>(T entity, WriteDbContext context) 
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
}