using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories.GuestRepository;

public interface IGuestRepository : IGenericRepository<Guest, GuestId>
{
    /// <summary>
    /// Gets all guests
    /// </summary>
    /// <returns>A list of all guests</returns>
    Task<IEnumerable<Guest>> GetAllAsync();
    
    /// <summary>
    /// Gets a guest by email
    /// </summary>
    /// <param name="email">ViaEmail</param>
    /// <returns>A guest</returns>
    Task<Guest> GetByEmailAsync(ViaEmail email);
}