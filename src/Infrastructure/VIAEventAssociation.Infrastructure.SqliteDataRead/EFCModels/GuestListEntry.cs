namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class GuestListEntry
{
    public int Id { get; set; }

    public string GuestId { get; set; } = null!;

    public string GuestListId { get; set; } = null!;

    public virtual GuestList GuestList { get; set; } = null!;
}
