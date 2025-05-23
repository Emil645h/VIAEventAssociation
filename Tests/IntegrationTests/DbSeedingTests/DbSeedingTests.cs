using VIAEventAssociation.Infrastructure.SqliteDataRead.Extensions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.DbSeedingTests;

public class DatabaseSeedingTests
{
    // Used to visualize and verify the data
    private readonly ITestOutputHelper _output;

    public DatabaseSeedingTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async Task DbHasGuestsSeed()
    {
        // Create seeded database in one line
        await using var ctx = DbContextSeedExtensions.SetupReadContext().Seed();
            
        // Assert
        Assert.NotEmpty(ctx.Guests);
        Assert.Equal(50, ctx.Guests.Count());
    }
        
    [Fact]
    public async Task DbHasEventsWithParticipants()
    {
        // Create seeded database
        await using var ctx = DbContextSeedExtensions.SetupReadContext().Seed();
            
        // Assert
        Assert.NotEmpty(ctx.Events);
        Assert.NotEmpty(ctx.GuestListEntries);
    }

    [Fact]
    public async Task DbHasEventsWithInvitations()
    {
        // Create seeded database
        await using var ctx = DbContextSeedExtensions.SetupReadContext().Seed();
        
        // Assert
        Assert.NotEmpty(ctx.Events);
        Assert.NotEmpty(ctx.Invites);
        
        var inviteCount = ctx.Invites.Count();
        var eventCount = ctx.Events.Count();
        
        _output.WriteLine($"Number of events: {eventCount}");
        _output.WriteLine($"Number of invites: {inviteCount}");
        
        foreach (var invite in ctx.Invites.Take(5)) // Show first 5
        {
            _output.WriteLine($"Invite ID: {invite.Id}, EventID: {invite.EventId}, Status: {invite.Status}");
        }
    }
        
    [Fact] 
    public async Task CanCreateEmptyDatabase()
    {
        // Create empty database (no .Seed() call)
        await using var ctx = DbContextSeedExtensions.SetupReadContext();
            
        // Assert
        Assert.Empty(ctx.Guests);
        Assert.Empty(ctx.Events);
    }
}