using Moq;
using UnitTests.Fakes;
using UnitTests.Fakes.Repositories;
using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Application.Features.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventHandlerTests
{
    private EventId eventId;
    private CreateEventCommand cmd;
    private FakeEventRepository eventRepo;
    private FakeUnitOfWork uow;
    private ICommandHandler<CreateEventCommand> handler;

    private void SetupBasic()
    {
        // Create an event ID and command
        var id = Guid.NewGuid().ToString();
        eventId = EventId.FromString(id).Value;
        cmd = CreateEventCommand.Create(id).Value;
        
        // Create repository and unit of work
        eventRepo = new FakeEventRepository();
        uow = new FakeUnitOfWork();
        
        // Create handler
        handler = new CreateEventHandler(eventRepo, uow);
    }
    
    [Fact]
    public async Task Handle_ValidInput_Success()
    {
        // Arrange
        SetupBasic();
        
        // Act
        var result = await handler.HandleAsync(cmd);
        
        // Assert
        Assert.IsAssignableFrom<Success<None>>(result);
        
        // Verify event was added to repository with correct properties
        var retrieveResult = await eventRepo.GetByIdAsync(eventId);
        var createdEvent = retrieveResult.Value;
        
        Assert.Equal("Working Title", createdEvent.title.Value);
        Assert.Equal("", createdEvent.description.Value);
        Assert.Equal(EventStatus.Draft, createdEvent.status);
        Assert.Equal(5, createdEvent.maxGuests.Value);
        Assert.Equal(EventVisibility.Private, createdEvent.visibility);
    }
    
    [Fact]
    public async Task Handle_GetNonExistingEvent_VerifiesNotInRepository()
    {
        // Arrange
        SetupBasic();
        
        // Act - execute the handler
        await handler.HandleAsync(cmd);
        
        // Get a different event ID that shouldn't exist
        var nonExistingId = EventId.FromString(Guid.NewGuid().ToString()).Value;
        var retrieveResult = await eventRepo.GetByIdAsync(nonExistingId);
        
        // Assert
        Assert.True(retrieveResult is Failure<VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event>);
        Assert.Equal(EventErrors.Repository.NotFound.Code, 
            ((Failure<VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event>)retrieveResult).Errors.First().Code);
    }
}