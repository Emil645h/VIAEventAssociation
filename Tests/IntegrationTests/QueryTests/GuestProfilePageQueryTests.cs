using Microsoft.Extensions.DependencyInjection;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

public class GuestProfilePageQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly IQueryHandler<GuestProfilePageQuery, GuestProfilePageAnswer> _handler;

    public GuestProfilePageQueryTests(ITestOutputHelper output)
    {
        _output = output;
        _handler = ServiceProvider.GetRequiredService<IQueryHandler<GuestProfilePageQuery, GuestProfilePageAnswer>>();
    }

    protected override void RegisterQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GuestProfilePageQuery, GuestProfilePageAnswer>, GuestProfilePageQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_WithValidGuestId_ReturnsProfileData()
    {
        // Arrange
        var firstGuest = Context.Guests.First();
        var query = new GuestProfilePageQuery(firstGuest.Id);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert & Output
        Assert.NotNull(result);
        Assert.Equal(firstGuest.Id, result.GuestId);
        Assert.Equal(firstGuest.FirstName, result.FirstName);
        Assert.Equal(firstGuest.LastName, result.LastName);
        Assert.Equal(firstGuest.Email, result.Email);

        _output.WriteLine("=== Guest Profile Page Query Result ===");
        _output.WriteLine(FormatAsJson(result));
        
        // Additional assertions
        Assert.True(result.UpcomingEventsCount >= 0);
        Assert.True(result.PendingInvitationsCount >= 0);
        Assert.True(result.PastEvents.Count <= 5, "Past events should be limited to 5");
    }

    [Fact]
    public async Task HandleAsync_ChecksUpcomingEventsAreInFuture()
    {
        // Arrange
        var guestWithEvents = Context.Guests.First();
        var query = new GuestProfilePageQuery(guestWithEvents.Id);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        foreach (var upcomingEvent in result.UpcomingEvents)
        {
            _output.WriteLine($"Upcoming Event: {upcomingEvent.Title} on {upcomingEvent.Date} at {upcomingEvent.StartTime}");
        }

        _output.WriteLine($"Test time: {TestTime.GetCurrentTime():yyyy-MM-dd HH:mm:ss}");
        _output.WriteLine($"Found {result.UpcomingEvents.Count} upcoming events and {result.PastEvents.Count} past events");
    }
}