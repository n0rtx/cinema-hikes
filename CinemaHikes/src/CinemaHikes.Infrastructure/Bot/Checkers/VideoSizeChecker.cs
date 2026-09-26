using CinemaHikes.Domain.Interfaces.Bot;

namespace CinemaHikes.Infrastructure.Bot.Checkers;

public sealed class VideoSizeChecker(HttpClient http) : IVideoSizeChecker
{
    private const long TelegramMaxBytes = 2L * 1024 * 1024 * 1024;

    private HttpClient Http { get; } = http;

    public async Task<bool> IsWithinTelegramLimitAsync(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (!response.IsSuccessStatusCode)
            return false;

        var contentLength = response.Content.Headers.ContentLength;
        if (contentLength is null)
            return false;

        return contentLength.Value <= TelegramMaxBytes;
    }
}