namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class GuestList
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<GuestListEntry> GuestListEntries { get; set; } = new List<GuestListEntry>();
}
