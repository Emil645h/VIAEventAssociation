using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate;

public static class LocationsErrors
{
    public static class LocationID
    {
        private const string LocationIdCode = "Location.Id";

        public static ResultError IsEmpty = new(LocationIdCode, "Location Id cannot be empty.");
    }

    public static class LocationName
    {
        private const string LocationNameCode = "Location.Name";
        
        public static ResultError IsEmpty = new(LocationNameCode, "Location Name cannot be empty.");
    }
}