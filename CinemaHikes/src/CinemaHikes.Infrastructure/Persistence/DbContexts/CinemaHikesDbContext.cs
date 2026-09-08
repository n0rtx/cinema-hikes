using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Parsing;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Persistence.DbContexts;

public class CinemaHikesDbContext(DbContextOptions<CinemaHikesDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
{
    public DbSet<Movie> Movies { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<MovieGenre> MovieGenres { get; set; }

    public DbSet<VideoSource> VideoSources { get; set; }

    public DbSet<TranslationStudio> TranslationStudios { get; set; }

    public DbSet<MovieLink> MovieLinks { get; set; }

    public DbSet<ParsingSourceConfig> ParsingSourceConfigs { get; set; }

    public DbSet<ViewHistoryEntry> ViewHistoryEntries { get; set; }

    public DbSet<FavoriteMovie> FavoriteMovies { get; set; }

    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.HasPostgresExtension("pg_trgm");
        
        builder.ApplyConfigurationsFromAssembly(typeof(CinemaHikesDbContext).Assembly);
    }
}