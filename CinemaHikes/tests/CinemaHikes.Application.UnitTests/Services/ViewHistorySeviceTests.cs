using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Entities.Users;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CinemaHikes.Application.UnitTests.Services;

public class ViewHistoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly ViewHistoryService _sut;

    public ViewHistoryServiceTests() => _sut = new ViewHistoryService(_uow.Object);

    [Fact]
    public async Task GetByUserIdAsync_ReturnsMappedHistory()
    {
        var items = new List<ViewHistoryEntry>
        {
            new()
            {
                Id = 1, UserId = 7, MovieId = 3, ProgressSeconds = 1200, CreatedAt = DateTime.UtcNow,
                Movie = new Movie
                {
                    Id = 3, RuTitle = "Фильм", UaTitle = "Ф", RuInEngTitle = "F",
                    Description = "d", Director = "d", ReleaseYear = 2019,
                    PosterUrl = "p", KpRating = 8, CreatedAt = DateTime.UtcNow,
                    MovieGenres = new List<MovieGenre>()
                }
            }
        };
        _uow.Setup(x => x.ViewHistory.GetByUserIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var result = await _sut.GetByUserIdAsync(7, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ProgressSeconds.Should().Be(1200);
    }

    [Fact]
    public async Task UpsertProgressAsync_WhenNew_AddsEntry()
    {
        _uow.Setup(x => x.ViewHistory.FindAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ViewHistoryEntry?)null);
        _uow.Setup(x => x.ViewHistory.AddAsync(It.IsAny<ViewHistoryEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.UpsertProgressAsync(1, 10, 500, CancellationToken.None);

        _uow.Verify(x => x.ViewHistory.AddAsync(
            It.Is<ViewHistoryEntry>(e => e.ProgressSeconds == 500),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertProgressAsync_WhenExists_UpdatesProgress()
    {
        var existing = new ViewHistoryEntry
            { UserId = 1, MovieId = 10, ProgressSeconds = 100, Movie = null!, CreatedAt = DateTime.UtcNow };
        _uow.Setup(x => x.ViewHistory.FindAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.UpsertProgressAsync(1, 10, 900, CancellationToken.None);

        existing.ProgressSeconds.Should().Be(900);
        _uow.Verify(x => x.ViewHistory.AddAsync(It.IsAny<ViewHistoryEntry>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_Empty_ReturnsEmpty()
    {
        _uow.Setup(x => x.ViewHistory.GetByUserIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ViewHistoryEntry>());

        var result = await _sut.GetByUserIdAsync(99, CancellationToken.None);

        result.Should().BeEmpty();
    }
}