using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;

namespace VIAEventAssociation.Infrastructure.SqliteDataWrite.Configurations;

public class GuestConfigurations : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(
                eId => eId.Value,
                dbValue => GuestId.FromGuid(dbValue).Value
            );

        builder.Property(guest => guest.firstName)
            .HasConversion(
                firstNameVo => firstNameVo.Value,
                firstName => FirstName.Create(firstName).Value
            ).HasColumnName("FirstName");

        builder.Property(guest => guest.lastName)
            .HasConversion(
                lastNameVo => lastNameVo.Value,
                lastName => LastName.Create(lastName).Value
            ).HasColumnName("LastName");

        builder.Property(guest => guest.profilePictureUrl)
            .HasConversion(
                profilePicUrlVo => profilePicUrlVo.Value,
                profilePic => ProfilePictureUrl.Create(profilePic.AbsoluteUri).Value
            ).HasColumnName("ProfilePictureUrl");

        builder.Property(guest => guest.email)
            .HasConversion(
                emailVo => emailVo.Value,
                email => ViaEmail.Create(email).Value
            ).HasColumnName("Email");
    }
}