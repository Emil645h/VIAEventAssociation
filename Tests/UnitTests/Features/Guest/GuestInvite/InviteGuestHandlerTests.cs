using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Guest;
using VIAEventAssociation.Core.Application.Features.Guest;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Guest.GuestInvite;

public class InviteGuestHandlerTests
{
    private readonly ICurrentTime _defaultTime = new StubCurrentTime(new DateTime(2026, 1, 1, 12, 0, 0));
    
    [Fact]
    public async Task GuestSuccessfullyInvitedToEvent()
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
        var title = EventTitle.Create("Valid Public Event").Value;
        @event.UpdateTitle(title);
        var startTime = new DateTime(2050, 1, 2, 12, 0, 0);
        var endTime = startTime.AddHours(3);
        var eventTime = EventTime.Create(startTime, endTime, _defaultTime).Value;
        @event.UpdateTime(eventTime);
        var maxGuests = EventMaxGuests.Create(20).Value;
        @event.UpdateMaxGuests(maxGuests);
        
        @event.SetActiveStatus(_defaultTime);

        var guestRepo = new FakeGuestRepository();
        var eventRepo = new FakeEventRepository();
        var uow = new FakeUnitOfWork();

        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(@event);

        var handler = new InviteGuestHandler(guestRepo, eventRepo, uow);
        var command = InviteGuestCommand.Create(eventId.ToString(), guest.Id.ToString()).Value;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result is Success<None>);
    }

}