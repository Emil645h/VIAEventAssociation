using Microsoft.Extensions.DependencyInjection;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

public class EventsEditingOverviewQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly IQueryHandler<EventsEditingOverviewQuery, EventsEditingOverviewAnswer> _handler;

    public EventsEditingOverviewQueryTests(ITestOutputHelper output)
    {
        _output = output;
        _handler = ServiceProvider.GetRequiredService<IQueryHandler<EventsEditingOverviewQuery, EventsEditingOverviewAnswer>>();
    }

    protected override void RegisterQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<EventsEditingOverviewQuery, EventsEditingOverviewAnswer>, EventsEditingOverviewQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsGroupedEventsByStatus()
    {
        // Arrange
        var query = new EventsEditingOverviewQuery();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);

        _output.WriteLine("=== Events Editing Overview Query Result ===");
        _output.WriteLine(FormatAsJson(result));

        _output.WriteLine($"📝 Draft Events: {result.TotalDrafts}");
        foreach (var draft in result.DraftEvents)
        {
            _output.WriteLine($"  - {draft.Title}");
        }

        _output.WriteLine($"✅ Ready Events: {result.TotalReady}");
        foreach (var ready in result.ReadyEvents)
        {
            _output.WriteLine($"  - {ready.Title}");
        }

        _output.WriteLine($"❌ Cancelled Events: {result.TotalCancelled}");
        foreach (var cancelled in result.CancelledEvents)
        {
            _output.WriteLine($"  - {cancelled.Title}");
        }

        // Verify totals match individual counts
        Assert.Equal(result.DraftEvents.Count, result.TotalDrafts);
        Assert.Equal(result.ReadyEvents.Count, result.TotalReady);
        Assert.Equal(result.CancelledEvents.Count, result.TotalCancelled);
    }
}