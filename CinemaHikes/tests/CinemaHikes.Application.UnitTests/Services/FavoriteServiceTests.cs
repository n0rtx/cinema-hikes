using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CinemaHikes.Application.UnitTests.Services;

public class FavoriteServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FavoriteService _sut;

    public FavoriteServiceTests() => _sut = new FavoriteService(_uow.Object);

    [Fact]
    public async Task GetByUserIdAsync_ReturnsMappedFavorites()
    {
        var favs = new List<FavoriteMovie>
        {
            new()
            {
                Id = 1, UserId = 10, MovieId = 5, CreatedAt = DateTime.UtcNow,
                Movie = new Movie
                {
                    Id = 5, RuTitle = "Фильм", UaTitle = "Ф", RuInEngTitle = "F",
                    Description = "d", Director = "d", ReleaseYear = 2020,
                    PosterUrl = "p", KpRating = 7, CreatedAt = DateTime.UtcNow,
                    MovieGenres = new List<MovieGenre>()
                }
            }
        };
        _uow.Setup(x => x.FavoriteMovies.GetByUserIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(favs);

        var result = await _sut.GetByUserIdAsync(10, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].MovieId.Should().Be(5);
        result[0].Movie.RuTitle.Should().Be("Фильм");
    }

    [Fact]
    public async Task AddAsync_WhenAlreadyExists_DoesNothing()
    {
        _uow.Setup(x => x.FavoriteMovies.FindAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FavoriteMovie
            {
                UserId = 1,
                MovieId = 2,
                Movie = null!,
                CreatedAt = DateTime.UtcNow
            });

        await _sut.AddAsync(1, 2, CancellationToken.None);

        _uow.Verify(x => x.FavoriteMovies.AddAsync(It.IsAny<FavoriteMovie>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WhenNotExists_AddsAndSaves()
    {
        _uow.Setup(x => x.FavoriteMovies.FindAsync(1, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FavoriteMovie?)null);
        _uow.Setup(x => x.FavoriteMovies.AddAsync(It.IsAny<FavoriteMovie>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.AddAsync(1, 3, CancellationToken.None);

        _uow.Verify(x => x.FavoriteMovies.AddAsync(
            It.Is<FavoriteMovie>(f => f.UserId == 1 && f.MovieId == 3),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenExists_RemovesAndSaves()
    {
        var existing = new FavoriteMovie
        {
            UserId = 1,
            MovieId = 5,
            Movie = null!,
            CreatedAt = DateTime.UtcNow
        };
        _uow.Setup(x => x.FavoriteMovies.FindAsync(1, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.RemoveAsync(1, 5, CancellationToken.None);

        _uow.Verify(x => x.FavoriteMovies.Remove(existing), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenNotExists_DoesNothing()
    {
        _uow.Setup(x => x.FavoriteMovies.FindAsync(1, 99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FavoriteMovie?)null);

        await _sut.RemoveAsync(1, 99, CancellationToken.None);

        _uow.Verify(x => x.FavoriteMovies.Remove(It.IsAny<FavoriteMovie>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}