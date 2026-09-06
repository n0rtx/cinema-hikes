using CinemaHikes.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class TranslationStudioEntityTypeConfiguration : IEntityTypeConfiguration<TranslationStudio>
{
    public void Configure(EntityTypeBuilder<TranslationStudio> builder)
    {
        builder.HasKey(ts => ts.Id);
        builder.Property(ts => ts.Id).ValueGeneratedOnAdd();

        builder.Property(ts => ts.Name)
            .HasMaxLength(170)
            .IsRequired();

        builder.HasIndex(ts => ts.Name)
            .IsUnique();

        builder.ToTable(ts => ts.HasCheckConstraint(
            name: "CK__TranslationStudio__Name",
            sql: $"LEN({nameof(TranslationStudio.Name)}) > 0"));
    }
}