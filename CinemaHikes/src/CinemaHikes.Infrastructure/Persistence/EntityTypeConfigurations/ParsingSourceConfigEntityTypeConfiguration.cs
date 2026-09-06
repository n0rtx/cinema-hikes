using CinemaHikes.Domain.Entities.Parsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class ParsingSourceConfigEntityTypeConfiguration : IEntityTypeConfiguration<ParsingSourceConfig>
{
    public void Configure(EntityTypeBuilder<ParsingSourceConfig> builder)
    {
        builder.HasKey(psc => psc.Id);
        builder.Property(psc => psc.Id).ValueGeneratedOnAdd();

        builder.Property(psc => psc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(psc => psc.BaseUrl)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(psc => psc.ParserType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(psc => psc.IsEnabled)
            .IsRequired();

        builder.HasIndex(psc => new { psc.Name, psc.BaseUrl })
            .IsUnique();

        builder.ToTable(psc => psc.HasCheckConstraint(
            name: "CK__ParsingSourceConfig__Name",
            sql: $"LEN({nameof(ParsingSourceConfig.Name)}) > 0"));
    }
}