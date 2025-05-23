using Microsoft.Extensions.DependencyInjection;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

public class ViewSingleEventQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly IQueryHandler<ViewSingleEventQuery, ViewSingleEventAnswer> _handler;

    public ViewSingleEventQueryTests(ITestOutputHelper output)
    {
        _output = output;
        _handler = ServiceProvider.GetRequiredService<IQueryHandler<ViewSingleEventQuery, ViewSingleEventAnswer>>();
    }

    protected override void RegisterQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<ViewSingleEventQuery, ViewSingleEventAnswer>, ViewSingleEventQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_WithValidEventId_ReturnsEventDetails()
    {
        // Arrange
        var firstEvent = Context.Events.First();
        var query = new ViewSingleEventQuery(firstEvent.Id, 0, 3, 3); // Show first 3 rows of 3 guests each

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(firstEvent.Id, result.EventId);
        Assert.Equal(firstEvent.Title, result.Title);

        _output.WriteLine("=== View Single Event Query Result ===");
        _output.WriteLine(FormatAsJson(result));

        _output.WriteLine($"Event: {result.Title}");
        _output.WriteLine($"Attendees: {result.CurrentAttendees}/{result.MaxAttendees}");
        _output.WriteLine($"Guests shown: {result.Guests.Count}");
        _output.WriteLine($"Total guests: {result.TotalGuests} across {result.TotalRows} rows");
    }

    [Fact]
    public async Task HandleAsync_RowBasedPagination_WorksCorrectly()
    {
        // Arrange - Find an event with multiple attendees
        var eventWithGuests = Context.Events
            .Where(e => Context.GuestListEntries.Any(gle => 
                Context.GuestLists.Any(gl => gl.EventId == e.Id && gl.Id == gle.GuestListId)))
            .First();

        var firstRowsQuery = new ViewSingleEventQuery(eventWithGuests.Id, 0, 3, 2); // Rows 0-1 (6 guests)
        var nextRowQuery = new ViewSingleEventQuery(eventWithGuests.Id, 1, 3, 2);   // Rows 1-2 (next 6 guests)

        // Act
        var firstResult = await _handler.HandleAsync(firstRowsQuery);
        var nextResult = await _handler.HandleAsync(nextRowQuery);

        // Assert
        Assert.Equal(0, firstResult.CurrentRowOffset);
        Assert.Equal(1, nextResult.CurrentRowOffset);

        _output.WriteLine("=== Row-based Pagination Test ===");
        _output.WriteLine($"Event: {firstResult.Title}");
        _output.WriteLine($"First query (rows 0-1): {firstResult.Guests.Count} guests");
        _output.WriteLine($"Next query (rows 1-2): {nextResult.Guests.Count} guests");
        
        foreach (var guest in firstResult.Guests)
        {
            _output.WriteLine($"First batch: {guest.FullName}");
        }
        foreach (var guest in nextResult.Guests)
        {
            _output.WriteLine($"Next batch: {guest.FullName}");
        }
    }
}