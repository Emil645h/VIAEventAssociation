using Microsoft.EntityFrameworkCore;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.EFCModels;

public partial class VeadatabaseProductionContext : DbContext
{
    public VeadatabaseProductionContext()
    {
    }

    public VeadatabaseProductionContext(DbContextOptions<VeadatabaseProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<GuestList> GuestLists { get; set; }

    public virtual DbSet<GuestListEntry> GuestListEntries { get; set; }

    public virtual DbSet<Invite> Invites { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source = /Users/emilbrugge/RiderProjects/VIAEventAssociation/src/Infrastructure/VIAEventAssociation.Infrastructure.SqliteDataWrite/VEADatabaseProduction.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuestList>(entity =>
        {
            entity.HasIndex(e => e.EventId, "IX_GuestLists_EventId").IsUnique();

            entity.HasOne(d => d.Event).WithOne(p => p.GuestList).HasForeignKey<GuestList>(d => d.EventId);
        });

        modelBuilder.Entity<GuestListEntry>(entity =>
        {
            entity.HasIndex(e => e.GuestListId, "IX_GuestListEntries_GuestListId");

            entity.HasOne(d => d.GuestList).WithMany(p => p.GuestListEntries).HasForeignKey(d => d.GuestListId);
        });

        modelBuilder.Entity<Invite>(entity =>
        {
            entity.ToTable("Invite");

            entity.HasIndex(e => e.EventId, "IX_Invite_EventId");

            entity.HasIndex(e => e.AssignedGuestId, "IX_Invite_assignedGuestId");

            entity.Property(e => e.AssignedGuestId).HasColumnName("assignedGuestId");

            entity.HasOne(d => d.AssignedGuest).WithMany(p => p.Invites).HasForeignKey(d => d.AssignedGuestId);

            entity.HasOne(d => d.Event).WithMany(p => p.Invites)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.HasIndex(e => e.EventId, "IX_Request_EventId");

            entity.HasIndex(e => e.AssignedGuestId, "IX_Request_assignedGuestId");

            entity.Property(e => e.AssignedGuestId).HasColumnName("assignedGuestId");

            entity.HasOne(d => d.AssignedGuest).WithMany(p => p.Requests).HasForeignKey(d => d.AssignedGuestId);

            entity.HasOne(d => d.Event).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
