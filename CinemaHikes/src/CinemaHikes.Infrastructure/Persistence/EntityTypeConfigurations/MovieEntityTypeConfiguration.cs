using CinemaHikes.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.RuTitle)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.UaTitle)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.RuInEngTitle)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(350)
            .IsRequired();

        builder.Property(m => m.Director)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.ReleaseYear)
            .IsRequired();

        builder.Property(m => m.PosterUrl)
            .HasMaxLength(350)
            .IsRequired();

        builder.Property(m => m.KpRating)
            .HasColumnType("numeric(3,1)")
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();
        
        builder.HasIndex(m => m.RuTitle)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(m => m.UaTitle)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(m => m.RuInEngTitle)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.ToTable(m =>
        {
            m.HasCheckConstraint(
                name: "CK__Movie__RuTitle",
                sql: $"LENGTH(\"{nameof(Movie.RuTitle)}\") > 0");
            m.HasCheckConstraint(
                name: "CK__Movie__UaTitle",
                sql: $"LENGTH(\"{nameof(Movie.UaTitle)}\") > 0");
            m.HasCheckConstraint(
                name: "CK__Movie__RuInEngTitle",
                sql: $"LENGTH(\"{nameof(Movie.RuInEngTitle)}\") > 0");
            m.HasCheckConstraint(
                name: "CK__Movie__Description",
                sql: $"LENGTH(\"{nameof(Movie.Description)}\") > 0");
            m.HasCheckConstraint(
                name: "CK__Movie__Director",
                sql: $"LENGTH(\"{nameof(Movie.Director)}\") > 0");
            m.HasCheckConstraint(
                name: "CK__Movie__PosterUrl",
                sql: $"LENGTH(\"{nameof(Movie.PosterUrl)}\") > 0");
        });
    }
}