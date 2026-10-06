using CinemaHikes.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class VideoSourceEntityTypeConfiguration : IEntityTypeConfiguration<VideoSource>
{
    public void Configure(EntityTypeBuilder<VideoSource> builder)
    {
        builder.HasKey(vs => vs.Id);
        builder.Property(vs => vs.Id).ValueGeneratedOnAdd();

        builder.HasOne(vs => vs.Movie)
            .WithMany(m => m.VideoSources)
            .HasForeignKey(vs => vs.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vs => vs.ParsingSourceConfig)
            .WithMany(psc => psc.VideoSources)
            .HasForeignKey(vs => vs.ParsingSourceConfigId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(vs => vs.ProviderName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(vs => vs.PageUrl)
            .HasMaxLength(400)
            .IsRequired();

        builder.Property(vs => vs.Priority)
            .IsRequired();

        builder.Property(vs => vs.Status)
            .IsRequired();
    }
}