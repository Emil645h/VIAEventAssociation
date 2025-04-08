using System.Diagnostics;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching.Dispatcher;

public class CommandExecutionTimer(ICommandDispatcher next) : ICommandDispatcher
{
    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        Result result = await next.DispatchAsync(command);
        
        TimeSpan elapsedTime = stopwatch.Elapsed;
        // do something with the time, log it, store in DB, whatever
        Console.WriteLine($"Command {typeof(TCommand).Name} executed in {elapsedTime.TotalMilliseconds}ms");
        
        return result;
    }
}