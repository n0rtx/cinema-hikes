namespace CinemaHikes.Domain.Interfaces.External;

public interface IOmdbClient
{
    Task<string?> GetPosterByTitleAndYearAsync(string title, short year, CancellationToken ct);
}