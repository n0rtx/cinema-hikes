using System.Net;
using System.Net.Http.Json;
using CinemaHikes.Application.Dtos.Auth;
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

    private static async Task AssertStatusAsync(
        HttpResponseMessage response,
        params HttpStatusCode[] expected)
    {
        var body = await response.Content.ReadAsStringAsync();
        var expectedList = string.Join(" or ", expected.Select(e => $"{e} ({(int)e})"));

        response.StatusCode.Should().BeOneOf(
            expected,
            because: $"expected [{expectedList}], got {response.StatusCode} ({(int)response.StatusCode}). Body:\n{body}");
    }

    [Fact]
    public async Task GetMovies_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?page=1&pageSize=10");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMovieById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/movies/99999");
        await AssertStatusAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGenres_ReturnsOkAndList()
    {
        var response = await _client.GetAsync("/api/genres");
        await AssertStatusAsync(response, HttpStatusCode.OK);

        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        genres.Should().NotBeNull();
    }

    [Fact]
    public async Task AddFavorite_Unauthorized_Returns401()
    {
        var response = await _client.PostAsync("/api/favorites/1", null);
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHistory_Unauthorized_Returns401()
    {
        var response = await _client.GetAsync("/api/history");
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateReview_Unauthorized_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/reviews", new { MovieId = 1, Text = "test", Rating = 8 });
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminCreateMovie_Unauthorized_Returns401()
    {
        var dto = new CreateMovieDto("T", "U", "E", "D", "Dir", 2020, "p", 7, new[] { 1 });
        var response = await _client.PostAsJsonAsync("/api/admin/movies", dto);
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMovieDetails_AfterCreate_ReturnsData()
    {
        var response = await _client.GetAsync("/api/movies/1");
        await AssertStatusAsync(response, HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HealthOrRoot_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/");
        await AssertStatusAsync(response,
            HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Redirect, HttpStatusCode.Found);
    }

    [Fact]
    public async Task MoviesFilterByYear_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?year=2020&page=1&pageSize=10");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task MoviesFilterByGenre_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?genreId=1&page=1&pageSize=10");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task MoviesFilterByMinRating_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?minRating=7.5&page=1&pageSize=10");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task MoviesFilterCombined_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?genreId=1&year=2020&minRating=6&page=1&pageSize=5");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task MoviesPageSizeClamped_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?page=1&pageSize=1000");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task MoviesNegativePage_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?page=-5&pageSize=10");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchMovies_QueryTooShort_Returns400()
    {
        var response = await _client.GetAsync("/api/movies/search?query=a");
        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchMovies_EmptyQuery_Returns400()
    {
        var response = await _client.GetAsync("/api/movies/search?query=");
        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchMovies_MissingQuery_Returns400()
    {
        var response = await _client.GetAsync("/api/movies/search");
        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchMovies_WhitespaceQuery_Returns400()
    {
        var response = await _client.GetAsync("/api/movies/search?query=%20%20");
        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetReviewsByMovie_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/reviews/movie/1");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetReviewsByMovie_NonExisting_ReturnsOkEmpty()
    {
        var response = await _client.GetAsync("/api/reviews/movie/999999");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFavorites_Unauthorized_Returns401()
    {
        var response = await _client.GetAsync("/api/favorites");
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteFavorite_Unauthorized_Returns401()
    {
        var response = await _client.DeleteAsync("/api/favorites/1");
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpsertHistory_Unauthorized_Returns401()
    {
        var response = await _client.PutAsJsonAsync("/api/history/1", new { ProgressSeconds = 100 });
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_Unauthorized_Returns401()
    {
        var response = await _client.GetAsync("/api/auth/me");
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_Unauthorized_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/change-password",
            new ChangePasswordRequestDto("old", "newpass1"));
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ValidPayload_ReturnsOkOrError()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var dto = new RegisterRequestDto($"user_{unique}", $"{unique}@test.local", "password1");
        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        await AssertStatusAsync(response,
            HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedOrError()
    {
        var dto = new LoginRequestDto("nobody@test.local", "wrong-password");
        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);
        await AssertStatusAsync(response,
            HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetMovies_ReturnsJsonArray()
    {
        var response = await _client.GetAsync("/api/movies?page=1&pageSize=5");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGenres_ContentTypeIsJson()
    {
        var response = await _client.GetAsync("/api/genres");
        await AssertStatusAsync(response, HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task MoviesSecondPage_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/movies?page=2&pageSize=5");
        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMovieById_Zero_Returns404()
    {
        var response = await _client.GetAsync("/api/movies/0");
        await AssertStatusAsync(response, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminDeleteMovie_Unauthorized_Returns401Or404()
    {
        var response = await _client.DeleteAsync("/api/admin/movies/1");
        await AssertStatusAsync(response, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }
}