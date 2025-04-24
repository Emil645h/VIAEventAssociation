using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

namespace SqliteDataWrite.Configurations;

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

        builder.ComplexProperty<InviteStatus>("inviteStatus",
            propBuilder =>
            {
                propBuilder.Property("_value")
                    .HasColumnName("StatusValue");

                propBuilder.Property("_displayName")
                    .HasColumnName("StatusDisplayName");
            });
    }
}