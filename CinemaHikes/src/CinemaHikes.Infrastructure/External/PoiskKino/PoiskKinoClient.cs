using System.Net.Http.Json;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Infrastructure.External.PoiskKino.JsonEntities;
using Microsoft.Extensions.Options;

namespace CinemaHikes.Infrastructure.External.PoiskKino;

public sealed class PoiskKinoClient : IMovieMetadataClient
{
    private HttpClient Http { get; }
    private PoiskKinoOptions Options { get; }

    public PoiskKinoClient(HttpClient http, IOptions<PoiskKinoOptions> options)
    {
        Http = http;
        Options = options.Value;

        if (string.IsNullOrWhiteSpace(Options.ApiKey))
            throw new InvalidOperationException("PoiskKino:ApiKey is missing. Set it via user-secrets.");

        Http.BaseAddress = new Uri(Options.BaseUrl.TrimEnd('/') + "/");
        Http.DefaultRequestHeaders.Remove("X-API-KEY");
        Http.DefaultRequestHeaders.Add("X-API-KEY", Options.ApiKey);
    }

    public async Task<IReadOnlyList<ExternalMovieData>> SearchAsync(string query, int limit, CancellationToken ct)
    {
        var url = $"v1.4/movie/search?query={Uri.EscapeDataString(query)}&limit={limit}&page=1";
        var response = await Http.GetFromJsonAsync<PoiskKinoSearchResponse>(url, ct);
        if (response?.Docs is null || response.Docs.Count == 0)
            return Array.Empty<ExternalMovieData>();

        return response.Docs.Select(PoiskKinoMapper.Map).ToList();
    }

    public async Task<ExternalMovieData?> GetByKinopoiskIdAsync(int kinopoiskId, CancellationToken ct)
    {
        var doc = await Http.GetFromJsonAsync<PoiskKinoMovieDoc>($"v1.4/movie/{kinopoiskId}", ct);
        return doc is null ? null : PoiskKinoMapper.Map(doc);
    }
}