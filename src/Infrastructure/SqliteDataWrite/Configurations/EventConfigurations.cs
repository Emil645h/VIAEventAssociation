using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.GuestList.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.LocationAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.Values;

namespace SqliteDataWrite.Configurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(
                eId => eId.Value,
                dbValue => EventId.FromGuid(dbValue).Value
            );

        builder.Property(e => e.title)
            .HasConversion(
                titleVo => titleVo.Value,
                title => EventTitle.Create(title).Value
            ).HasColumnName("Title");

        builder.Property(e => e.description)
            .HasConversion(
                descVo => descVo.Value,
                desc => EventDescription.Create(desc).Value
            ).HasColumnName("Description");

        builder.Property(e => e.maxGuests)
            .HasConversion(
                maxGuestsVo => maxGuestsVo.Value,
                maxGuests => EventMaxGuests.Create(maxGuests).Value
            ).HasColumnName("MaxGuests");
        
        builder.ComplexProperty<EventVisibility>("visibility",
            propBuilder =>
            {
                propBuilder.Property("_value")
                    .HasColumnName("VisibilityValue");
                propBuilder.Property("_displayName")
                    .HasColumnName("VisibilityName");
            }
        );
        
        builder.ComplexProperty<EventStatus>("status",
            propBuilder =>
            {
                propBuilder.Property("_value")
                    .HasColumnName("StatusValue");
                propBuilder.Property("_displayName")
                    .HasColumnName("StatusName");
            }
        );

        builder.OwnsOne<EventTime>("eventTime", eventTimeBuilder =>
        {
            eventTimeBuilder.Property(eventTime => eventTime.StartTime)
                .HasColumnName("StartTime");
            
            eventTimeBuilder.Property(eventTime => eventTime.EndTime)
                .HasColumnName("EndTime");
        });
        
        builder.OwnsOne(e => e.guestList, guestListBuilder => {
            guestListBuilder.ToTable("GuestLists");
            
            guestListBuilder.WithOwner().HasForeignKey("EventId");
            
            guestListBuilder.HasKey(gl => gl.Id);
            
            guestListBuilder
                .Property(gl => gl.Id)
                .HasConversion(
                    glId => glId.Value,
                    dbValue => GuestListId.Create(dbValue).Value
                );
            
            guestListBuilder.Ignore(gl => gl.numberOfGuests);
            
            guestListBuilder
                .OwnsMany<GuestId>("guests", guestsBuilder => {
                    guestsBuilder.ToTable("GuestListEntries");
                    
                    // Create a joining table
                    guestsBuilder.WithOwner().HasForeignKey("GuestListId");
                    
                    guestsBuilder.Property<int>("Id").ValueGeneratedOnAdd();
                    guestsBuilder.HasKey("Id");
                    
                    guestsBuilder
                        .Property(guestId => guestId.Value)
                        .HasColumnName("GuestId");
                });
        });
        
        builder
            .HasMany(e => e.invites)
            .WithOne()
            .HasForeignKey("EventId")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(e => e.requests)
            .WithOne()
            .HasForeignKey("EventId")
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}