using VIAEventAssociation.Core.QueryContracts.Contract;

namespace VIAEventAssociation.Core.QueryContracts.Queries;

public record ViewSingleEventQuery(
    string EventId,
    int RowOffset,        // Which row to start from (0-based)
    int GuestsPerRow,     // Guests per row (e.g., 3)
    int RowsToShow        // How many rows to show (e.g., 3 for 9 guests total)
) : IQuery<ViewSingleEventAnswer>;

public record ViewSingleEventAnswer(
    string EventId,
    string Title,
    string Description,
    string Location,
    string Date,
    string StartTime,
    string EndTime,
    bool IsPublic,
    string Visibility,
    int CurrentAttendees,
    int MaxAttendees,
    List<EventGuestInfo> Guests,
    int CurrentRowOffset,
    int TotalGuests,
    int TotalRows,
    bool HasMoreRows
);

public record EventGuestInfo(
    string GuestId,
    string FirstName,
    string LastName,
    string FullName,
    string ProfilePictureUrl
);