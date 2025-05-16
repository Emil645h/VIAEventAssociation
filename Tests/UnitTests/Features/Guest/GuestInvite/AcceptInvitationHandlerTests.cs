using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class AcceptInvitationHandlerTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));

    [Fact]
    public async Task GuestSuccessfullyAcceptsInvitation()
    {
        // Arrange
        var guest = VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.Guest.Create(
            GuestId.Create(Guid.NewGuid()).Value,
            FirstName.Create("Emil").Value,
            LastName.Create("Invited").Value,
            ViaEmail.Create("emil@via.dk").Value,
            ProfilePictureUrl.Create("https://img").Value
        ).Value;

        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var @event = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId).Value;

        @event.UpdateTitle(EventTitle.Create("Active Event").Value);
        var start = _defaultTime.GetCurrentTime().AddHours(1);
        var end = start.AddHours(3);
        @event.UpdateTime(EventTime.Create(start, end, _defaultTime).Value);
        @event.UpdateMaxGuests(EventMaxGuests.Create(5).Value);
        @event.MakePublic();
        @event.SetActiveStatus(_defaultTime);

        
        @event.CreateGuestInvite(guest);

        
        var guestRepo = new FakeGuestRepository();
        var eventRepo = new FakeEventRepository();
        var uow = new FakeUnitOfWork();

        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(@event);

        var handler = new AcceptInvitationHandler(guestRepo, eventRepo, uow, _defaultTime);
        var command = AcceptInvitationCommand.Create(eventId.ToString(), guest.Id.ToString()).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
    }
}
