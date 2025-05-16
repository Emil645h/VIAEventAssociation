using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Invite;
using VIAEventAssociation.Core.Application.Features.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class DeclineInvitationHandlerTests
{
    [Fact]
    public async Task GuestSuccessfullyDeclinesInvitation()
    {
        // Arrange
        var guest = VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.Guest.Create(
            GuestId.Create(Guid.NewGuid()).Value,
            FirstName.Create("Emil").Value,
            LastName.Create("Decliner").Value,
            ViaEmail.Create("123456@via.dk").Value,
            ProfilePictureUrl.Create("https://img.jpg").Value
        ).Value;

        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var @event = VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event.Create(eventId).Value;

        var currentTime = new StubCurrentTime(new DateTime(2050, 1, 1, 12, 0, 0));

        @event.UpdateTitle(EventTitle.Create("Declinable Event").Value);
        @event.UpdateDescription(EventDescription.Create("Test desc").Value);

        var start = currentTime.GetCurrentTime().AddHours(1);
        var end = start.AddHours(3);
        var eventTime = EventTime.Create(start, end, currentTime).Value;
        @event.UpdateTime(eventTime);

        @event.UpdateMaxGuests(EventMaxGuests.Create(20).Value);
        @event.MakePublic();
        @event.SetActiveStatus(currentTime);

        @event.CreateGuestInvite(guest);

        var guestRepo = new FakeGuestRepository();
        var eventRepo = new FakeEventRepository();
        var uow = new FakeUnitOfWork();

        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(@event);

        var handler = new DeclineInvitationHandler(guestRepo, eventRepo, uow);
        var command = DeclineInvitationCommand.Create(eventId.ToString(), guest.Id.ToString()).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
    }
}
