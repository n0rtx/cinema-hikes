namespace CinemaHikes.Domain.Interfaces.External;

public interface IMovieMetadataClient
{
    Task<IReadOnlyList<ExternalMovieData>> SearchAsync(string query, int limit, CancellationToken ct);
    Task<ExternalMovieData?> GetByKinopoiskIdAsync(int kinopoiskId, CancellationToken ct);
}