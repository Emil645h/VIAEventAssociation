using VIAEventAssociation.Core.Application.CommandDispatching;
using VIAEventAssociation.Core.Application.CommandDispatching.Commands.Event;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Application.Features.Event;

public class ActivateEventHandler : ICommandHandler<ActivateEventCommand>
{
    private readonly IEventRepository eventRepo;
    private readonly IUnitOfWork uow;
    private readonly ICurrentTime currentTime;

    internal ActivateEventHandler(IEventRepository eventRepo, IUnitOfWork uow, ICurrentTime currentTime)
    {
        this.eventRepo = eventRepo;
        this.uow = uow;
        this.currentTime = currentTime;
    }

    public async Task<Result> HandleAsync(ActivateEventCommand command)
    {
        // Get the event
        var getResult = await eventRepo.GetByIdAsync(command.Id);
        if (getResult.IsFailure)
            return getResult;

        var @event = getResult.Value;
        
        // If the event is in draft state, first try to make it ready
        var eventStatus = GetEventStatus(@event);
        if (eventStatus == EventStatus.Draft)
        {
            var readyResult = @event.SetReadyStatus(currentTime);
            if (readyResult.IsFailure)
                return readyResult;
        }
        
        // Now activate the event
        var activateResult = @event.SetActiveStatus(currentTime);
        if (activateResult.IsFailure)
            return activateResult;

        // Save changes
        await uow.SaveChangesAsync();
        return new Success<None>(new None());
    }
    
    // Helper method to get event status through reflection (since status is internal)
    private EventStatus GetEventStatus(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event @event)
    {
        var statusField = typeof(VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Event)
            .GetField("status", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        return (EventStatus)statusField.GetValue(@event);
    }
}