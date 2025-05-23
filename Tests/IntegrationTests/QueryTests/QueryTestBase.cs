using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Extensions;

namespace IntegrationTests.QueryTests;

public abstract class QueryTestBase : IDisposable
{
    protected readonly VeadatabaseProductionContext Context;
    protected readonly StubCurrentTime TestTime;
    protected readonly IServiceProvider ServiceProvider;

    protected QueryTestBase()
    {
        // Set test time to middle of data range (15/03/2024 at 12:00)
        TestTime = new StubCurrentTime(new DateTime(2024, 3, 15, 12, 0, 0));
        
        // Setup context with seeded data
        Context = DbContextSeedExtensions.SetupReadContext().Seed();
        
        // Setup service collection for dependency injection
        var services = new ServiceCollection();
        services.AddSingleton<ICurrentTime>(TestTime);
        services.AddSingleton(Context);
        
        RegisterQueryHandlers(services);
        
        ServiceProvider = services.BuildServiceProvider();
    }

    protected abstract void RegisterQueryHandlers(IServiceCollection services);

    protected static string FormatAsJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public void Dispose()
    {
        Context?.Dispose();
        
        // Cast to IDisposable since the concrete ServiceProvider implements it
        if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
    }
}