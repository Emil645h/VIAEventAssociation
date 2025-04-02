using System.Runtime.InteropServices.JavaScript;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Fakes.Repositories;

public class FakeEventRepository : IEventRepository
{
    private readonly Dictionary<Guid, Event> _events = new();

    public Task<Result<None>> AddAsync(Event @event)
    {
        _events[@event.Id.Value] = @event;
        return Task.FromResult<Result<None>>(new None());
    }

    public Task<Result<Event>> GetByIdAsync(EventId id)
    {
        if (_events.TryGetValue(id.Value, out var @event))
        {
            return Task.FromResult<Result<Event>>(@event);
        }

        return Task.FromResult<Result<Event>>(EventErrors.Repository.NotFound);
    }

    public Task<Result<None>> RemoveAsync(EventId id)
    {
        _events.Remove(id.Value);
        return Task.FromResult<Result<None>>(new None());
    }

    public Task<Result<None>> UpdateAsync(Event @event)
    {
        _events[@event.Id.Value] = @event;
        return Task.FromResult<Result<None>>(new None());
    }
}