using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories.EventRepository;

public interface IEventRepository : IGenericRepository<Event, EventId>
{
    /// <summary>
    /// Gets all events
    /// </summary>
    /// <returns>A list of Events</returns>
    Task<IEnumerable<Event>> GetAllAsync();
}