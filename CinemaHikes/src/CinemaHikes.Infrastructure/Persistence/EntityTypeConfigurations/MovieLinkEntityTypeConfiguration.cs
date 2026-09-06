using CinemaHikes.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class MovieLinkEntityTypeConfiguration : IEntityTypeConfiguration<MovieLink>
{
    public void Configure(EntityTypeBuilder<MovieLink> builder)
    {
        builder.HasKey(ml => ml.Id);
        builder.Property(ml => ml.Id).ValueGeneratedOnAdd();

        builder.HasOne(ml => ml.Movie)
            .WithMany(m => m.MovieLinks)
            .HasForeignKey(ml => ml.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ml => ml.TranslationStudio)
            .WithMany()
            .HasForeignKey(ml => ml.TranslationStudioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ml => ml.VideoQuality)
            .IsRequired();

        builder.Property(ml => ml.Url)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(ml => ml.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(ml => new { ml.MovieId, ml.TranslationStudioId, ml.VideoQuality })
            .IsUnique();
    }
}