using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Fakes.Repositories;

public class FakeGuestRepository : IGuestRepository
{
    private readonly Dictionary<Guid, Guest> _guests = new();

    public Task<Result<None>> AddAsync(Guest guest)
    {
        _guests[guest.Id.Value] = guest;
        return Task.FromResult<Result<None>>(new Success<None>(new None()));
    }

    public Task<Result<Guest>> GetByIdAsync(GuestId id)
    {
        if (_guests.TryGetValue(id.Value, out var guest))
        {
            return Task.FromResult<Result<Guest>>(new Success<Guest>(guest));
        }

        return Task.FromResult<Result<Guest>>(new Failure<Guest>(new[] { new ResultError("Guest.Repository", "Guest not found") }));
    }

    public Task<Result<None>> RemoveAsync(GuestId id)
    {
        _guests.Remove(id.Value);
        return Task.FromResult<Result<None>>(new None());
    }

    public Task<Result<None>> UpdateAsync(Guest guest)
    {
        _guests[guest.Id.Value] = guest;
        return Task.FromResult<Result<None>>(new None());
    }
    
    public int Count => _guests.Count;
}