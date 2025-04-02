using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;

public interface IEventRepository
{
    /// <summary>
    /// Adds a new event to the repository
    /// </summary>
    /// <param name="event">The event to add</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> AddAsync(Event @event);
    
    /// <summary>
    /// Gets an event by its ID
    /// </summary>
    /// <param name="id">The ID of the event to retrieve</param>
    /// <returns>The requested event or an error</returns>
    Task<Result<Event>> GetByIdAsync(EventId id);
    
    /// <summary>
    /// Removes an event from the repository
    /// </summary>
    /// <param name="id">The ID of the event to remove</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> RemoveAsync(EventId id);
    
    /// <summary>
    /// Updates an existing event in the repository
    /// </summary>
    /// <param name="event">The event with the updated information</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> UpdateAsync(Event @event);
}