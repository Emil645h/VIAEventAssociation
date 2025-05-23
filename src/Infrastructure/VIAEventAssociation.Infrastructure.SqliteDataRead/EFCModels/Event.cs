namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class Event
{
    public string Id { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int MaxGuests { get; set; }

    public string Title { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Visibility { get; set; } = null!;

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public virtual GuestList? GuestList { get; set; }

    public virtual ICollection<Invite> Invites { get; set; } = new List<Invite>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
