using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;

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

    public static class AssignToGuestList
    {
        private const string AssignToGuestListCode = "Event.GuestList.AssignToGuestList";
        
        public static ResultError GuestIsEmpty = new(AssignToGuestListCode, "Guest Id cannot be empty.");
        public static ResultError GuestAlreadyAssigned = new(AssignToGuestListCode, "Guest Id is already assigned to guest list.");
    }
}