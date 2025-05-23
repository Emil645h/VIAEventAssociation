using VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Extensions;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

/// <summary>
/// This test class is used for testing the dates. There's previously been issues with the dates.
/// </summary>
public class DateFormatDiagnosticTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly VeadatabaseProductionContext _context;

    public DateFormatDiagnosticTests(ITestOutputHelper output)
    {
        _output = output;
        _context = DbContextSeedExtensions.SetupReadContext().Seed();
    }

    [Fact]
    public void DiagnoseEventDateFields()
    {
        var events = _context.Events.Take(5).ToList();
        
        _output.WriteLine("=== Event Date Field Diagnosis ===");
        foreach (var evt in events)
        {
            _output.WriteLine($"Event: {evt.Title}");
            _output.WriteLine($"  StartTime field: '{evt.StartTime}'");
            _output.WriteLine($"  EndTime field: '{evt.EndTime}'");
            _output.WriteLine($"  Status: '{evt.Status}'");
            _output.WriteLine($"  Visibility: '{evt.Visibility}'");
            _output.WriteLine("");
        }
    }

    [Fact]
    public void CheckSpecificEventData()
    {
        // Check "Garden Games" event from your data
        var gardenGamesEvent = _context.Events
            .FirstOrDefault(e => e.Id == "23a28a9a-2380-468d-9afc-c5cc1cda66f5");

        if (gardenGamesEvent != null)
        {
            _output.WriteLine("=== Garden Games Event ===");
            _output.WriteLine($"Title: {gardenGamesEvent.Title}");
            _output.WriteLine($"Start: {gardenGamesEvent.StartTime}");
            _output.WriteLine($"End: {gardenGamesEvent.EndTime}");
            _output.WriteLine($"Status: {gardenGamesEvent.Status}");
            _output.WriteLine($"Visibility: {gardenGamesEvent.Visibility}");
        }
        else
        {
            _output.WriteLine("Garden Games event not found!");
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}