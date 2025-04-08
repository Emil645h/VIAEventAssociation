using VIAEventAssociation.Core.Tools.OperationResult.CustomExceptions;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Dispatcher;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Type serviceType = typeof(ICommandHandler<TCommand>);
        var service = serviceProvider.GetService(serviceType);
        if (service == null)
        {
            throw new ServiceNotFoundException(nameof(ICommandHandler<TCommand>));
        }
        
        ICommandHandler<TCommand> handler = (ICommandHandler<TCommand>)service;
        return handler.HandleAsync(command);
    }
}