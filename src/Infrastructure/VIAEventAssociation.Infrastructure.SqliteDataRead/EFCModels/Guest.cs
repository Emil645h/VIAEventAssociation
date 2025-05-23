namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class Guest
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfilePictureUrl { get; set; } = null!;

    public virtual ICollection<Invite> Invites { get; set; } = new List<Invite>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
