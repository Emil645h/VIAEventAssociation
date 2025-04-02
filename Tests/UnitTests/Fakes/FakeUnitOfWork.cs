using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Fakes;

/// <summary>
/// A fake implementation of IUnitOfWork for testing purposes
/// </summary>
public class FakeUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Fake implementation that just returns a completed task with success
    /// </summary>
    /// <returns>A completed task with success result</returns>
    public Task<Result<None>> SaveChangesAsync()
    {
        return Task.FromResult<Result<None>>(new None());
    }
}