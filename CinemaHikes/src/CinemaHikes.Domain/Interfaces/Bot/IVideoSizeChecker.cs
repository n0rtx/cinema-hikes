namespace CinemaHikes.Domain.Interfaces.Bot;

public interface IVideoSizeChecker
{
    Task<bool> IsWithinTelegramLimitAsync(string url, CancellationToken ct = default);
}