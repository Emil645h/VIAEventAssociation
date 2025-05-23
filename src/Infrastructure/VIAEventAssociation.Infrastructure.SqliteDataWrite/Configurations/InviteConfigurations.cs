using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Configurations;

public class InviteConfigurations : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                iId => iId.Value,
                dbValue => InviteId.FromGuid(dbValue).Value
            );
        
        builder.HasOne<Guest>().WithMany().HasForeignKey("assignedGuestId");

        builder.ComplexProperty<InviteStatus>("inviteStatus",
            statusBuilder =>
            {
                statusBuilder.Ignore("_value");
                statusBuilder.Property("_displayName")
                    .HasColumnName("Status");
            });
    }
}