using Microsoft.Extensions.DependencyInjection;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

public class UpcomingEventsPageQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly IQueryHandler<UpcomingEventsPageQuery, UpcomingEventsPageAnswer> _handler;

    public UpcomingEventsPageQueryTests(ITestOutputHelper output)
    {
        _output = output;
        _handler = ServiceProvider.GetRequiredService<IQueryHandler<UpcomingEventsPageQuery, UpcomingEventsPageAnswer>>();
    }

    protected override void RegisterQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<UpcomingEventsPageQuery, UpcomingEventsPageAnswer>, UpcomingEventsPageQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_FirstPage_ReturnsUpcomingEvents()
    {
        // Arrange
        var query = new UpcomingEventsPageQuery(null, 1, 5);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CurrentPage == 1);
        Assert.True(result.TotalPages >= 1);

        _output.WriteLine("=== Upcoming Events Page Query Result ===");
        _output.WriteLine($"Page {result.CurrentPage} of {result.TotalPages}");
        _output.WriteLine($"Total events: {result.TotalEvents}");
        _output.WriteLine(FormatAsJson(result));
    }

    [Fact]
    public async Task HandleAsync_WithSearchText_FiltersEvents()
    {
        // Arrange - search for events containing "party"
        var searchQuery = new UpcomingEventsPageQuery("Scary Movie Night", 1, 10);
        var allEventsQuery = new UpcomingEventsPageQuery(null, 1, 10);

        // Act
        var searchResult = await _handler.HandleAsync(searchQuery);
        var allResult = await _handler.HandleAsync(allEventsQuery);

        // Assert
        Assert.True(searchResult.TotalEvents <= allResult.TotalEvents);

        _output.WriteLine("=== Search Results ===");
        _output.WriteLine($"Search term: 'Scary Movie Night'");
        _output.WriteLine($"Found {searchResult.TotalEvents} events (out of {allResult.TotalEvents} total)");
        
        foreach (var eventItem in searchResult.Events)
        {
            _output.WriteLine($"- {eventItem.Title}");
            Assert.Contains("party", eventItem.Title, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task HandleAsync_Pagination_WorksCorrectly()
    {
        // Arrange
        var page1Query = new UpcomingEventsPageQuery(null, 1, 2);
        var page2Query = new UpcomingEventsPageQuery(null, 2, 2);

        // Act
        var page1Result = await _handler.HandleAsync(page1Query);
        var page2Result = await _handler.HandleAsync(page2Query);

        // Assert
        Assert.Equal(1, page1Result.CurrentPage);
        Assert.Equal(2, page2Result.CurrentPage);
        Assert.Equal(page1Result.TotalEvents, page2Result.TotalEvents);
        Assert.Equal(page1Result.TotalPages, page2Result.TotalPages);

        _output.WriteLine("=== Pagination Test ===");
        _output.WriteLine($"Total events: {page1Result.TotalEvents}, Total pages: {page1Result.TotalPages}");
        _output.WriteLine("Page 1 events:");
        foreach (var evt in page1Result.Events)
        {
            _output.WriteLine($"- {evt.Title}");
        }
        _output.WriteLine("Page 2 events:");
        foreach (var evt in page2Result.Events)
        {
            _output.WriteLine($"- {evt.Title}");
        }
    }
}