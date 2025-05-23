using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Request.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Configurations;

public class RequestConfigurations : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                rId => rId.Value,
                dbValue => RequestId.FromGuid(dbValue).Value
            );
        
        builder.HasOne<Guest>().WithMany().HasForeignKey("assignedGuestId");
        
        builder.ComplexProperty<RequestStatus>("requestStatus",
            statusBuilder =>
            {
                statusBuilder.Ignore("_value");
                statusBuilder.Property("_displayName")
                    .HasColumnName("Status");
            });

        builder.Property<string>("reason")
            .HasColumnName("Reason");
    }
}