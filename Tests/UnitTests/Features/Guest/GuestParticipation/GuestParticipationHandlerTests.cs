using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.Participation;

public class GuestParticipationHandlerTests
{
    private VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event @event;
    private VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.Guest guest;
    private GuestParticipationCommand command;
    private FakeEventRepository eventRepo;
    private FakeGuestRepository guestRepo;
    private FakeUnitOfWork uow;
    private ICurrentTime time;
    private ICommandHandler<GuestParticipationCommand> handler;

    [Fact]
    public async Task GuestSuccessfullyParticipatesInPublicActiveEvent()
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

        @event.UpdateTitle(EventTitle.Create("Test Event").Value);
        @event.UpdateDescription(EventDescription.Create("For testing").Value);

        var start = now.AddHours(2);
        var end = start.AddHours(3);
        var eventTime = EventTime.Create(start, end, currentTime).Value;
        @event.UpdateTime(eventTime);

        @event.UpdateMaxGuests(EventMaxGuests.Create(10).Value);
        @event.MakePublic();
        @event.SetActiveStatus(currentTime);

        var eventRepo = new FakeEventRepository();
        var guestRepo = new FakeGuestRepository();
        var uow = new FakeUnitOfWork();

        await eventRepo.AddAsync(@event);
        await guestRepo.AddAsync(guest);

        var command = GuestParticipationCommand.Create(
            eventId.ToString(),
            guest.Id.ToString()
        ).Value;

        var handler = new GuestParticipationHandler(guestRepo, eventRepo, uow, currentTime);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
    }

}
