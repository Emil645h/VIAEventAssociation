using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace SqliteDataWrite.UnitOfWork;

public class SqliteUnitOfWork(WriteDbContext writeDbContext) : IUnitOfWork
{
    public async Task<Result<None>> SaveChangesAsync()
    {
        await writeDbContext.SaveChangesAsync();
        return new None();
    }
}