namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class Invite
{
    public string Id { get; set; } = null!;

    public string? EventId { get; set; }

    public string AssignedGuestId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Guest AssignedGuest { get; set; } = null!;

    public virtual Event? Event { get; set; }
}
