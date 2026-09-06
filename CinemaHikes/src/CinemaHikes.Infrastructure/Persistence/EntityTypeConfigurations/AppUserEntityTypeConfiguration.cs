using CinemaHikes.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class AppUserEntityTypeConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.RegisteredAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(u => u.AmountOfHats)
            .HasDefaultValue(0);
    }
}