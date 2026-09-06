using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaHikes.Infrastructure.Persistence.EntityTypeConfigurations;

public sealed class FavoriteMovieEntityTypeConfiguration : IEntityTypeConfiguration<FavoriteMovie>
{
    public void Configure(EntityTypeBuilder<FavoriteMovie> builder)
    {
        builder.HasKey(fm => fm.Id);
        builder.Property(fm => fm.Id).ValueGeneratedOnAdd();

        builder.HasOne(fm => fm.Movie)
            .WithMany(m => m.FavoriteMovies)
            .HasForeignKey(fm => fm.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(fm => fm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(fm => fm.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(fm => new { fm.MovieId, fm.UserId })
            .IsUnique();
    }
}