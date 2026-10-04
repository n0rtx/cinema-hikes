using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace CinemaHikes.Application.UnitTests.Services;

public class MovieServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMovieMetadataClient> _metadata = new();
    private readonly MovieService _sut;

    public MovieServiceTests()
    {
        _sut = new MovieService(_uow.Object, _metadata.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsMappedList()
    {
        var movies = new List<Movie>
        {
            CreateMovie(1, "Матрица", 1999),
            CreateMovie(2, "Интерстеллар", 2014)
        };
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movies);

        var result = await _sut.GetPagedAsync(null, null, null, 1, 10, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].RuTitle.Should().Be("Матрица");
        result[1].ReleaseYear.Should().Be(2014);
    }

    [Fact]
    public async Task GetPagedAsync_WithGenreFilter_PassesSpec()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        await _sut.GetPagedAsync(5, 2020, 7.5, 1, 20, CancellationToken.None);

        _uow.Verify(x => x.Movies.ListAsync(
            It.Is<MovieFilterSpecification>(s => true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenLocalFound_DoesNotCallExternal()
    {
        var local = new List<Movie> { CreateMovie(1, "Дюна", 2021) };
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);

        var result = await _sut.SearchAsync("Дюна", 1, 10, CancellationToken.None);

        result.Should().HaveCount(1);
        _metadata.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WhenLocalEmpty_CallsExternalAndSaves()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var external = new List<ExternalMovieData>
        {
            new(1,"Новый фильм", "Новий", "New Film", "Desc", "Director", 2024, "poster.jpg", 8.1, new[] { "Боевик" })
        };
        _metadata.Setup(x => x.SearchAsync("Новый", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(external);

        _uow.Setup(x => x.Genres.GetByNameAsync("Боевик", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);
        _uow.Setup(x => x.Genres.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _sut.SearchAsync("Новый", 1, 10, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].RuTitle.Should().Be("Новый фильм");
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDetailsAsync_WhenExists_ReturnsDetails()
    {
        var movie = CreateMovie(42, "Брат", 1997);
        movie.Description = "Крутой фильм";
        movie.Director = "Балабанов";
        _uow.Setup(x => x.Movies.GetWithDetailsAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movie);

        var result = await _sut.GetDetailsAsync(42, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Description.Should().Be("Крутой фильм");
        result.Director.Should().Be("Балабанов");
    }

    [Fact]
    public async Task GetDetailsAsync_WhenNotFound_ReturnsNull()
    {
        _uow.Setup(x => x.Movies.GetWithDetailsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie?)null);

        var result = await _sut.GetDetailsAsync(999, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ExternalWithExistingGenre_ReusesGenre()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var external = new List<ExternalMovieData>
        {
            new(2, "Фильм", "Фільм", "Film", "D", "Dir", 2023, "p.jpg", 7.0, new[] { "Драма" })
        };
        _metadata.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(external);

        var existingGenre = new Genre { Id = 3, Name = "Драма" };
        _uow.Setup(x => x.Genres.GetByNameAsync("Драма", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGenre);
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.SearchAsync("Фильм", 1, 5, CancellationToken.None);

        _uow.Verify(x => x.Genres.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_EmptyResult_ReturnsEmptyList()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var result = await _sut.GetPagedAsync(1, 2020, 8.0, 1, 10, CancellationToken.None);

        result.Should().BeEmpty();
    }

    private static Movie CreateMovie(int id, string title, short year) => new()
    {
        Id = id,
        RuTitle = title,
        UaTitle = title,
        RuInEngTitle = title,
        Description = "desc",
        Director = "dir",
        ReleaseYear = year,
        PosterUrl = "url",
        KpRating = 8.0,
        CreatedAt = DateTime.UtcNow,
        MovieGenres = new List<MovieGenre>()
    };
}