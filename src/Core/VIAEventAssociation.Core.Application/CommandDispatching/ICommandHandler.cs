using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.CommandDispatching;

/// <summary>
/// Defines the contract for handling commands in the application.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle</typeparam>
public interface ICommandHandler<TCommand>
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <returns>A result indicating success or failure</returns>
    Task<Result> HandleAsync(TCommand command);
}