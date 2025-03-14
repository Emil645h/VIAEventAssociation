using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Locations;

public static class LocationErrors
{
    public static class LocationId
    {
        public const string LocationIdCode = "Location ID cannot be empty";
        
        public static ResultError IsEmpty = new(LocationIdCode, "Location ID cannot be empty.");
    }
}