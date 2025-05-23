using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories;

/// <summary>
/// Base repository implementation using Entity Framework Core
/// </summary>
/// <typeparam name="TAgg">The aggregate type</typeparam>
/// <typeparam name="TId">The ID type</typeparam>
public abstract class RepositoryEfcBase<TAgg, TId>(WriteDbContext context) : IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot<TId>
    where TId : class
{
    public virtual async Task<TAgg> GetAsync(TId id)
    {
        var aggregate = await FindByIdAsync(id);

        if (aggregate == null)
            throw new KeyNotFoundException($"{typeof(TAgg).Name} with ID {id} was not found");
        
        return aggregate;
    }

    public virtual async Task RemoveAsync(TId id)
    {
        var aggregate = await FindByIdAsync(id);
        
        if (aggregate == null)
            throw new KeyNotFoundException($"{typeof(TAgg).Name} with ID {id} was not found");
        
        context.Set<TAgg>().Remove(aggregate);
    }

    public virtual async Task AddAsync(TAgg aggregate)
    {
        await context.Set<TAgg>().AddAsync(aggregate);
    }

    /// <summary>
    /// Finds an aggregate by ID, including any related entities.
    /// Derived classes should override this to include related entities
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The aggregate found or null</returns>
    protected virtual async Task<TAgg> FindByIdAsync(TId id)
    {
        return await context.Set<TAgg>().FindAsync(id);
    }

}