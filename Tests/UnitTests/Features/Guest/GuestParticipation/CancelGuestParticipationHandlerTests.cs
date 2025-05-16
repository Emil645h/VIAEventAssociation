using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestParticipation;

public class CancelGuestParticipationHandlerTests
{
    [Fact]
    public async Task GuestSuccessfullyCancelsParticipationBeforeEventStart()
    {
        // Arrange
        var now = new DateTime(2050, 1, 1, 12, 0, 0);
        var currentTime = new StubCurrentTime(now);

        var guest = VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.Guest.Create(
            GuestId.Create(Guid.NewGuid()).Value,
            FirstName.Create("Emil").Value,
            LastName.Create("Test").Value,
            ViaEmail.Create("emil@via.dk").Value,
            ProfilePictureUrl.Create("https://image").Value
        ).Value;

        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var @event = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId).Value;
        @event.UpdateTitle(EventTitle.Create("Event").Value);
        @event.UpdateDescription(EventDescription.Create("Desc").Value);

        var start = now.AddHours(2);
        var end = start.AddHours(3);
        @event.UpdateTime(EventTime.Create(start, end, currentTime).Value);
        @event.UpdateMaxGuests(EventMaxGuests.Create(10).Value);
        @event.MakePublic();
        @event.SetActiveStatus(currentTime);

        @event.AssignGuestToGuestList(guest, currentTime);

        var eventRepo = new FakeEventRepository();
        var guestRepo = new FakeGuestRepository();
        await eventRepo.AddAsync(@event);
        await guestRepo.AddAsync(guest);
        var uow = new FakeUnitOfWork();

        var handler = new CancelGuestParticipationHandler(guestRepo, eventRepo, uow, currentTime);
        var command = CancelGuestParticipationCommand.Create(
            eventId.ToString(),
            guest.Id.ToString()
        ).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
    }
}
