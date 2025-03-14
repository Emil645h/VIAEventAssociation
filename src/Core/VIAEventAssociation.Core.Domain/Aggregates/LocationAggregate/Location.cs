using VIAEventAssociation.Core.Domain.Aggregates.Locations.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Locations;

public class Location : AggregateRoot<LocationId>
{
    
    internal LocationName locationName;
    internal DateTime availableStart;
    internal DateTime availableEnd;
    internal int capacity;
    
    private Location(LocationId id, LocationName locationName, DateTime availableStart, DateTime availableEnd, int capacity) : base(id) 
        => (this.locationName, this.availableStart, this.availableEnd, this.capacity) = (locationName, availableStart, availableEnd, capacity);
    
    public static Result<Location> Create(LocationId id, LocationName locationName, DateTime availableStart, DateTime availableEnd, int capacity) 
        => new Location(id, locationName, availableStart, availableEnd, capacity);

}