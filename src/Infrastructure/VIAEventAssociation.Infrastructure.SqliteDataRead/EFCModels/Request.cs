namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class Request
{
    public string Id { get; set; } = null!;

    public string? EventId { get; set; }

    public string? AssignedGuestId { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Guest? AssignedGuest { get; set; }

    public virtual Event? Event { get; set; }
}
