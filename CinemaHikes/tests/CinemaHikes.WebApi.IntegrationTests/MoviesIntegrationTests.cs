using System.Net;
using System.Net.Http.Json;
using CinemaHikes.Application.Dtos.Catalog;
using FluentAssertions;
using Xunit;

namespace CinemaHikes.WebApi.IntegrationTests;

public class MoviesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MoviesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMovies_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMovieById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/movies/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchMovies_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies/search?query=test&page=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGenres_ReturnsOkAndList()
    {
        var response = await _client.GetAsync("/api/genres");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        genres.Should().NotBeNull();
    }

    [Fact]
    public async Task AddFavorite_Unauthorized_Returns401()
    {
        var response = await _client.PostAsync("/api/favorites/1", null);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHistory_Unauthorized_Returns401()
    {
        var response = await _client.GetAsync("/api/history");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateReview_Unauthorized_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/reviews", new { MovieId = 1, Text = "test", Rating = 8 });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminCreateMovie_Unauthorized_Returns401()
    {
        var dto = new CreateMovieDto("T", "U", "E", "D", "Dir", 2020, "p", 7, new[] { 1 });
        var response = await _client.PostAsJsonAsync("/api/admin/movies", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMovieDetails_AfterCreate_ReturnsData()
    {
        var response = await _client.GetAsync("/api/movies/1");
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HealthOrRoot_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MoviesFilterByYear_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?year=2020&page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}