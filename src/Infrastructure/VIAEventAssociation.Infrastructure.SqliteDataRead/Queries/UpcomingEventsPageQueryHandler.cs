using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Common;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;

public class UpcomingEventsPageQueryHandler : IQueryHandler<UpcomingEventsPageQuery, UpcomingEventsPageAnswer>
{
    private readonly VeadatabaseProductionContext _context;
    private readonly ICurrentTime _currentTime;

    public UpcomingEventsPageQueryHandler(VeadatabaseProductionContext context, ICurrentTime currentTime)
    {
        _context = context;
        _currentTime = currentTime;
    }

    public async Task<UpcomingEventsPageAnswer> HandleAsync(UpcomingEventsPageQuery query)
    {
        // Get current time for filtering upcoming events
        string currentTime = _currentTime.GetCurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");

        // Build the base query for upcoming events
        var baseQuery = _context.Events
            .Where(e => e.StartTime != null && e.StartTime.CompareTo(currentTime) > 0);

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            baseQuery = baseQuery.Where(e => e.Title.Contains(query.SearchText));
        }

        // Order by start time (earliest first)
        var orderedQuery = baseQuery.OrderBy(e => e.StartTime);

        // Get total count for pagination
        var totalEvents = await orderedQuery.CountAsync();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling((double)totalEvents / query.PageSize);

        // Apply pagination
        var eventsData = await orderedQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Description,
                e.StartTime,
                e.MaxGuests,
                e.Visibility,
                AttendeesCount = _context.GuestListEntries
                    .Where(gle => _context.GuestLists
                        .Where(gl => gl.EventId == e.Id)
                        .Select(gl => gl.Id)
                        .Contains(gle.GuestListId))
                    .Count()
            })
            .ToListAsync();

        // Transform to result format
        var eventItems = eventsData.Select(e => new UpcomingEventItem(
            e.Id,
            e.Title,
            TruncateDescription(e.Description, 100), // Limit description length
            DateTimeFormatHelper.FormatDateForProfile(e.StartTime),
            DateTimeFormatHelper.FormatTime(e.StartTime),
            e.AttendeesCount,
            e.MaxGuests,
            e.Visibility.Equals("Public", StringComparison.OrdinalIgnoreCase),
            e.Visibility.Equals("Private", StringComparison.OrdinalIgnoreCase)
        )).ToList();

        return new UpcomingEventsPageAnswer(
            eventItems,
            query.PageNumber,
            Math.Max(totalPages, 1), // Ensure at least 1 page
            totalEvents
        );
    }

    private static string TruncateDescription(string description, int maxLength)
    {
        if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
            return description ?? "";

        return description.Substring(0, maxLength).TrimEnd() + "...";
    }
}