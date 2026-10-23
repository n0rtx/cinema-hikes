using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Bot;
using CinemaHikes.Domain.Interfaces.External;
using CinemaHikes.Domain.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace CinemaHikes.Application.UnitTests.Services;

public class MovieServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMovieMetadataClient> _metadata = new();
    private readonly Mock<IOmdbClient> _omdb = new();
    private readonly Mock<UserManager<AppUser>> _userManager;
    private readonly Mock<IMovieParserFacade> _parser = new();
    private readonly MovieService _sut;

    public MovieServiceTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManager = new Mock<UserManager<AppUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _sut = new MovieService(
            _uow.Object,
            _metadata.Object,
            _omdb.Object,
            _userManager.Object,
            _parser.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsMappedList()
    {
        var movies = new List<Movie>
        {
            CreateMovie(1, "Матрица", 1999),
            CreateMovie(2, "Интерстеллар", 2014)
        };

        _uow.Setup(x => x.Movies.CountAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(movies);

        var result = await _sut.GetPagedAsync(
            genreIds: null,
            year: null,
            minRating: null,
            sort: null,
            matchAllGenres: false,
            page: 1,
            pageSize: 10,
            onlyWithPoster: false,
            ct: CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Items[0].RuTitle.Should().Be("Матрица");
        result.Items[1].ReleaseYear.Should().Be(2014);
    }

    [Fact]
    public async Task GetPagedAsync_WithGenreFilter_PassesSpec()
    {
        _uow.Setup(x => x.Movies.CountAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        await _sut.GetPagedAsync(
            genreIds: new[] { 5 },
            year: 2020,
            minRating: 7.5,
            sort: "rating",
            matchAllGenres: false,
            page: 1,
            pageSize: 20,
            onlyWithPoster: false,
            ct: CancellationToken.None);

        _uow.Verify(x => x.Movies.ListAsync(
            It.IsAny<MovieFilterSpecification>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.Movies.CountAsync(
            It.IsAny<MovieFilterSpecification>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithMultipleGenres_PassesSpec()
    {
        _uow.Setup(x => x.Movies.CountAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        await _sut.GetPagedAsync(
            genreIds: new[] { 1, 5, 12 },
            year: null,
            minRating: null,
            sort: null,
            matchAllGenres: false,
            page: 1,
            pageSize: 24,
            onlyWithPoster: false,
            ct: CancellationToken.None);

        _uow.Verify(x => x.Movies.ListAsync(
            It.IsAny<MovieFilterSpecification>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_EmptyResult_ReturnsEmptyList()
    {
        _uow.Setup(x => x.Movies.CountAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var result = await _sut.GetPagedAsync(
            genreIds: new[] { 1 },
            year: 2020,
            minRating: 8.0,
            sort: null,
            matchAllGenres: false,
            page: 1,
            pageSize: 10,
            onlyWithPoster: false,
            ct: CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task SearchAsync_WhenLocalFound_DoesNotCallExternal()
    {
        var local = new List<Movie> { CreateMovie(1, "Дюна", 2021) };
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(local);
        _uow.Setup(x => x.Movies.CountAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _sut.SearchAsync("Дюна", 1, 10, userId: null, ct: CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Total.Should().Be(1);
        _metadata.Verify(
            x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WhenLocalEmpty_CallsExternalAndSaves()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var external = new List<ExternalMovieData>
        {
            new(1, "Новый фильм", "Новий", "New Film", "Desc", "Director", 2024, "poster.jpg", 8.1,
                new[] { "Боевик" })
        };
        _metadata.Setup(x => x.SearchAsync("Новый", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(external);

        _uow.Setup(x => x.Genres.ListAsync(It.IsAny<EmptySpecification<Genre>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre>());
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _omdb.Setup(x => x.GetPosterByTitleAndYearAsync(
                It.IsAny<string>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var result = await _sut.SearchAsync("Новый", 1, 10, userId: null, ct: CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].RuTitle.Should().Be("Новый фильм");
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WhenLocalEmpty_AndUserIdProvided_AwardsHat()
    {
        _uow.Setup(x => x.Movies.ListAsync(It.IsAny<MovieSearchSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var external = new List<ExternalMovieData>
        {
            new(1, "Новый фильм", "Новий", "New Film", "Desc", "Director", 2024, "poster.jpg", 8.1,
                new[] { "Боевик" })
        };
        _metadata.Setup(x => x.SearchAsync("Новый", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(external);

        _uow.Setup(x => x.Genres.ListAsync(It.IsAny<EmptySpecification<Genre>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre>());
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _omdb.Setup(x => x.GetPosterByTitleAndYearAsync(
                It.IsAny<string>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var user = new AppUser
        {
            Id = 7,
            UserName = "tester",
            Email = "t@t.com",
            RegisteredAt = DateTime.UtcNow,
            AmountOfHats = 3
        };
        _userManager.Setup(x => x.FindByIdAsync("7")).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _sut.SearchAsync("Новый", 1, 10, userId: 7, ct: CancellationToken.None);

        result.Items.Should().HaveCount(1);
        user.AmountOfHats.Should().Be(4);
        _userManager.Verify(x => x.UpdateAsync(user), Times.Once);
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
        _uow.Setup(x => x.Genres.ListAsync(It.IsAny<EmptySpecification<Genre>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre> { existingGenre });
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _omdb.Setup(x => x.GetPosterByTitleAndYearAsync(
                It.IsAny<string>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        await _sut.SearchAsync("Фильм", 1, 5, userId: null, ct: CancellationToken.None);

        _uow.Verify(x => x.Movies.AddAsync(
            It.Is<Movie>(m => m.MovieGenres.Any(mg => mg.GenreId == 3 || mg.Genre.Id == 3)),
            It.IsAny<CancellationToken>()), Times.Once);
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

    private static Movie CreateMovie(int id, string title, short year) => new()
    {
        Id = id,
        RuTitle = title,
        UaTitle = title + " UA",
        RuInEngTitle = title + " Eng",
        Description = "desc",
        Director = "dir",
        ReleaseYear = year,
        PosterUrl = "url",
        KpRating = 8.0,
        CreatedAt = DateTime.UtcNow,
        MovieGenres = new List<MovieGenre>()
    };
}