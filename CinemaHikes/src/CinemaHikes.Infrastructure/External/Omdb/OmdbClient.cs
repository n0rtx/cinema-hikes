using System.Net.Http.Json;
using CinemaHikes.Domain.Interfaces.External;
using Microsoft.Extensions.Options;

namespace CinemaHikes.Infrastructure.External.Omdb;

public sealed class OmdbClient : IOmdbClient
{
    private readonly HttpClient _http;
    private readonly OmdbOptions _options;

    public OmdbClient(HttpClient http, IOptions<OmdbOptions> options)
    {
        _http = http;
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("Omdb:ApiKey is missing. Set it via user-secrets.");

        _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<string?> GetPosterByTitleAndYearAsync(string title, short year, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(title) || year <= 0)
            return null;

        var url =
            $"{_options.BaseUrl.TrimEnd('/')}/" +
            $"?apikey={Uri.EscapeDataString(_options.ApiKey)}" +
            $"&t={Uri.EscapeDataString(title.Trim())}" +
            $"&y={year}" +
            "&type=movie";

        try
        {
            var response = await _http.GetFromJsonAsync<OmdbPosterResponse>(url, ct);
            if (response is null
                || !string.Equals(response.Response, "True", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(response.Poster)
                || string.Equals(response.Poster, "N/A", StringComparison.OrdinalIgnoreCase))
                return null;

            return response.Poster;
        }
        catch
        {
            return null;
        }
    }
}