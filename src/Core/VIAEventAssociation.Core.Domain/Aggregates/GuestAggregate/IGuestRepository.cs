using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

public interface IGuestRepository
{
    /// <summary>
    /// Adds a new guest to the repository
    /// </summary>
    /// <param name="guest">The guest to add</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> AddAsync(Guest guest);
    
    /// <summary>
    /// Gets a guest by their ID
    /// </summary>
    /// <param name="id">The ID of the guest to retrieve</param>
    /// <returns>The requested guest or an error</returns>
    Task<Result<Guest>> GetByIdAsync(GuestId id);
    
    /// <summary>
    /// Removes a guest from the repository
    /// </summary>
    /// <param name="id">The ID of the guest to remove</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> RemoveAsync(GuestId id);
    
    /// <summary>
    /// Updates an existing guest in the repository
    /// </summary>
    /// <param name="guest">The guest with updated information</param>
    /// <returns>Success or error result</returns>
    Task<Result<None>> UpdateAsync(Guest guest);
}