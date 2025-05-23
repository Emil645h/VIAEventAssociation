using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories;

/// <summary>
/// Generic repository interface for all aggregates
/// </summary>
/// <typeparam name="TAgg">The aggregate type</typeparam>
/// <typeparam name="TId">The ID type</typeparam>
public interface IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot<TId>
    where TId : class
{
    /// <summary>
    /// Gets an aggregate by ID
    /// </summary>
    Task<TAgg> GetAsync(TId id);
    
    /// <summary>
    /// Removes an aggregate by ID
    /// </summary>
    Task RemoveAsync(TId id);
    
    /// <summary>
    /// Adds a new aggregate
    /// </summary>
    Task AddAsync(TAgg aggregate);
}