using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.GuestList;

public static class GuestListErrors
{
    public static class GuestListId
    {
        private const string GuestListIdCode = "Event.GuestList.Id";
        
        public static ResultError IsEmpty = new(GuestListIdCode, "Guest List Id cannot be empty.");
    }

    public static class NumberOfGuests
    {
        private const string NumberOfGuestsCode = "Event.GuestList.NumberOfGuests";
        
        public static ResultError IsInvalidRange = new(NumberOfGuestsCode, "Guest List Number cannot be less than 0 or larger than 100.");
    }
}