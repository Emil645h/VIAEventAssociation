using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;

public class EventsEditingOverviewQueryHandler : IQueryHandler<EventsEditingOverviewQuery, EventsEditingOverviewAnswer>
{
    private readonly VeadatabaseProductionContext _context;

    public EventsEditingOverviewQueryHandler(VeadatabaseProductionContext context)
    {
        _context = context;
    }

    public async Task<EventsEditingOverviewAnswer> HandleAsync(EventsEditingOverviewQuery query)
    {
        // Get all events from the system
        var allEvents = await _context.Events
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Status
            })
            .ToListAsync();

        // Group events by status - doing this in memory to avoid multiple DB calls
        var draftEvents = allEvents
            .Where(e => e.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.Title)
            .Select(e => new EventSummary(e.Id, e.Title))
            .ToList();

        var readyEvents = allEvents
            .Where(e => e.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase) || 
                        e.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.Title)
            .Select(e => new EventSummary(e.Id, e.Title))
            .ToList();

        var cancelledEvents = allEvents
            .Where(e => e.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.Title)
            .Select(e => new EventSummary(e.Id, e.Title))
            .ToList();

        return new EventsEditingOverviewAnswer(
            draftEvents,
            readyEvents,
            cancelledEvents,
            draftEvents.Count,
            readyEvents.Count,
            cancelledEvents.Count
        );
    }
}