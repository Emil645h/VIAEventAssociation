using Microsoft.EntityFrameworkCore;
using SqliteDataWrite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace IntegrationTests.DbContextConfigurations;

public class WriteDbContextTests
{
    [Fact]
    public void SetupContext_CreatesNewDatabaseEachTime()
    {
        // Act
        using var context1 = WriteDbContext.SetupContext();
        using var context2 = WriteDbContext.SetupContext();
        
        // Assert
        Assert.NotEqual(context1.Database.GetConnectionString(), context2.Database.GetConnectionString());
    }
    
    [Fact]
    public async Task CanSaveAndRetrieveEvent()
    {
        // Arrange
        using var context = WriteDbContext.SetupContext();
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var event1 = Event.Create(eventId).Value;
        
        // Act
        await context.Events.AddAsync(event1);
        await context.SaveChangesAsync();
        
        // Clear the change tracker to ensure we're getting a fresh read
        context.ChangeTracker.Clear();
        
        var retrievedEvent = await context.Events.FindAsync(eventId);
        
        // Assert
        Assert.NotNull(retrievedEvent);
        Assert.Equal(eventId.Value, retrievedEvent.Id.Value);
    }
    
    [Fact]
    public async Task CanSaveAndRetrieveGuest()
    {
        // Arrange
        using var context = WriteDbContext.SetupContext();
        var guestId = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePic = ProfilePictureUrl.Create("https://example.com/profile.jpg").Value;
        
        var guest = Guest.Create(guestId, firstName, lastName, email, profilePic).Value;
        
        // Act
        await context.Guests.AddAsync(guest);
        await context.SaveChangesAsync();
        
        // Clear the change tracker
        context.ChangeTracker.Clear();
        
        var retrievedGuest = await context.Guests.FindAsync(guestId);
        
        // Assert
        Assert.NotNull(retrievedGuest);
        Assert.Equal(guestId.Value, retrievedGuest.Id.Value);
        Assert.Equal(firstName.Value, retrievedGuest.firstName.Value);
    }
}

public class WriteDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_ReturnsValidContext()
    {
        // Arrange
        var factory = new WriteDbContextFactory();
        
        // Act
        using var context = factory.CreateDbContext(Array.Empty<string>());
        
        // Assert
        Assert.NotNull(context);
        Assert.Contains("VEADatabaseProduction.db", context.Database.GetConnectionString());
    }
    
    [Fact]
    public void CreateDbContext_ConfiguresSqliteDatabase()
    {
        // Arrange
        var factory = new WriteDbContextFactory();
        
        // Act
        using var context = factory.CreateDbContext(Array.Empty<string>());
        
        // Assert
        Assert.Contains("Sqlite", context.Database.ProviderName);
    }
    
    [Fact(Skip = "This test would create the actual production database file")]
    public void CreateDbContext_CanEnsureCreated()
    {
        // Arrange
        var factory = new WriteDbContextFactory();
        
        // Act
        using var context = factory.CreateDbContext(Array.Empty<string>());
        bool created = context.Database.EnsureCreated();
        
        // Clean up - delete the file after testing
        if (File.Exists("VEADatabaseProduction.db"))
        {
            File.Delete("VEADatabaseProduction.db");
        }
        
        // Assert
        Assert.True(created);
    }
}