using System.Reflection;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Dispatcher;

namespace VIAEventAssociation.Core.Application.Extensions;

public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registers all command handlers from the specified assembly
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assembly">The assembly to scan for handlers (defaults to the calling assembly)</param>
    /// <returns>The service collection for method chaining</returns>
    // public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly = null)
    // {
    //     // If no assembly is provided, use the Application assembly
    //     assembly ??= typeof(ApplicationServiceExtensions).Assembly;
    //
    //     // Register the CommandDispatcher
    //     services.AddScoped<CommandDispatcher>();
    //
    //     // Register the CommandExecutionTimer decorator
    //     services.AddScoped<ICommandDispatcher>(provider =>
    //         new CommandExecutionTimer(provider.GetRequiredService<CommandDispatcher>()));
    //
    //     // Find all non-abstract classes that implement ICommandHandler<T> (for any T)
    //     var handlerTypes = assembly.GetTypes()
    //         .Where(t => t.IsClass && !t.IsAbstract)
    //         .Where(t => t.GetInterfaces().Any(i =>
    //             i.IsGenericType &&
    //             i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));
    //
    //     foreach (var handlerType in handlerTypes)
    //     {
    //         // Get the ICommandHandler<T> interface that this class implements
    //         var commandHandlerInterface = handlerType.GetInterfaces()
    //             .First(i => i.IsGenericType &&
    //                         i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
    //
    //         // Register the handler with its interface
    //         services.AddScoped(commandHandlerInterface, handlerType);
    //
    //         Console.WriteLine($"Registered handler: {handlerType.Name} as {commandHandlerInterface.Name}");
    //     }
    //
    //     return services;
    // }
}