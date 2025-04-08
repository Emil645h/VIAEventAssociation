using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching;

public interface ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command);
}