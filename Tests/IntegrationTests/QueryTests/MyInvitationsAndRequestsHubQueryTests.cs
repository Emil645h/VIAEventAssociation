using Microsoft.Extensions.DependencyInjection;
using VIAEventAssociation.Core.QueryContracts.Contract;
using VIAEventAssociation.Core.QueryContracts.Queries;
using VIAEventAssociation.Infrastructure.SqliteDataRead.Queries;
using Xunit.Abstractions;

namespace IntegrationTests.QueryTests;

public class MyInvitationsAndRequestsHubQueryTests : QueryTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly IQueryHandler<MyInvitationsAndRequestsHubQuery, MyInvitationsAndRequestsHubAnswer> _handler;

    public MyInvitationsAndRequestsHubQueryTests(ITestOutputHelper output)
    {
        _output = output;
        _handler = ServiceProvider.GetRequiredService<IQueryHandler<MyInvitationsAndRequestsHubQuery, MyInvitationsAndRequestsHubAnswer>>();
    }

    protected override void RegisterQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<MyInvitationsAndRequestsHubQuery, MyInvitationsAndRequestsHubAnswer>, MyInvitationsAndRequestsHubQueryHandler>();
    }

    [Fact]
    public async Task HandleAsync_WithValidGuestId_ReturnsHubData()
    {
        // Arrange - Find a guest who has invitations or requests
        var guestWithInvites = Context.Invites.Select(i => i.AssignedGuestId).FirstOrDefault();
        if (guestWithInvites == null)
        {
            guestWithInvites = Context.Guests.First().Id; // Fallback to first guest
        }

        var query = new MyInvitationsAndRequestsHubQuery(guestWithInvites);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);

        _output.WriteLine("=== My Invitations & Requests Hub Query Result ===");
        _output.WriteLine(FormatAsJson(result));

        _output.WriteLine($"🔔 {result.PendingInvitationsCount} Pending Invitations");
        foreach (var invitation in result.PendingInvitations)
        {
            _output.WriteLine($"  - {invitation.EventTitle} on {invitation.EventDate} at {invitation.EventTime}");
            _output.WriteLine($"    Attendees: {invitation.CurrentAttendees}/{invitation.MaxAttendees}");
        }

        _output.WriteLine($"⏳ {result.PendingRequestsCount} Pending Requests");
        foreach (var request in result.PendingRequests)
        {
            _output.WriteLine($"  - {request.EventTitle} (Requested: {request.RequestedDate})");
        }

        _output.WriteLine($"📋 {result.RecentActivityCount} Recent Activities");
        foreach (var activity in result.RecentActivities)
        {
            _output.WriteLine($"  - {activity.Description}");
        }

        // Verify counts match list sizes
        Assert.Equal(result.PendingInvitations.Count, result.PendingInvitationsCount);
        Assert.Equal(result.PendingRequests.Count, result.PendingRequestsCount);
        Assert.Equal(result.RecentActivities.Count, result.RecentActivityCount);
    }
}