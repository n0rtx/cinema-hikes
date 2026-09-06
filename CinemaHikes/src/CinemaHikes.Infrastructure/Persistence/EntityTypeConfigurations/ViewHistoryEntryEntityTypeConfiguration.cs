using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class ViewHistoryEntryEntityTypeConfiguration : IEntityTypeConfiguration<ViewHistoryEntry>
{
    public void Configure(EntityTypeBuilder<ViewHistoryEntry> builder)
    {
        builder.HasKey(vh => vh.Id);
        builder.Property(vh => vh.Id).ValueGeneratedOnAdd();

        builder.HasOne(vh => vh.Movie)
            .WithMany(m => m.ViewHistoryEntries)
            .HasForeignKey(vh => vh.Movie)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(vh => vh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(vh => vh.ProgressSeconds)
            .HasDefaultValue(0);

        builder.Property(vh => vh.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}