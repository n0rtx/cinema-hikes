using CinemaHikes.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class GenreEntityTypeConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedOnAdd();

        builder.Property(g => g.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.ToTable(g => g.HasCheckConstraint(
            name: "CK__Genre__Name",
            sql: $"LENGTH(\"{nameof(Genre.Name)}\") > 0"));
    }
}