using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Common.UnitOfWork;

/// <summary>
/// Represents the Unit of Work pattern interface.
/// Coordinates the work of multiple repositories by ensuring that all repositories use the same 
/// database context and that all operations are completed (committed) successfully as a unit.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made in the current transaction context to the database.
    /// </summary>
    /// <returns>A result indicating success or failure of the operation</returns>
    Task<Result<None>> SaveChangesAsync();
}