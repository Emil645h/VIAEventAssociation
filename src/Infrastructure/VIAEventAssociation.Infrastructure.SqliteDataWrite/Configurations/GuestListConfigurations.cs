using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Configurations;

public class GuestListConfigurations : IEntityTypeConfiguration<GuestList>
{
    public void Configure(EntityTypeBuilder<GuestList> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(g => g.Id)
            .HasConversion(
                gId => gId.Value,
                dbValue => GuestListId.FromGuid(dbValue).Value
            );
        
        builder.Ignore(g => g.numberOfGuests);

        // For private collection field, specify the exact type
        builder
            .OwnsMany<GuestId>("guests", guestsBuilder => {
                guestsBuilder.ToTable("GuestListEntries");
                
                // Create a joining table
                guestsBuilder.WithOwner().HasForeignKey("GuestListId");
                
                // We need a primary key for the join table
                guestsBuilder.Property<int>("Id").ValueGeneratedOnAdd();
                guestsBuilder.HasKey("Id");
                
                // Map the GuestId value object - needs to access its Value property
                guestsBuilder
                    .Property(guestId => guestId.Value)
                    .HasColumnName("GuestId");
            });
    }
}