using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CinemaHikes.Application.UnitTests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly ReviewService _sut;

    public ReviewServiceTests() => _sut = new ReviewService(_uow.Object);

    [Fact]
    public async Task GetByMovieIdAsync_ReturnsOnlyApproved()
    {
        var reviews = new List<Review>
        {
            new()
            {
                Id = 1,
                MovieId = 10,
                UserId = 1,
                Text = "Отлично",
                Rating = 9,
                Status = ReviewStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                Movie = null!
            }
        };
        _uow.Setup(x => x.Reviews.GetByMovieIdAsync(10, ReviewStatus.Approved, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);

        var result = await _sut.GetByMovieIdAsync(10, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(ReviewStatus.Approved);
        result[0].Text.Should().Be("Отлично");
    }

    [Fact]
    public async Task CreateAsync_AddsWithPendingStatus()
    {
        _uow.Setup(x => x.Reviews.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _sut.CreateAsync(5, 20, "Хороший фильм", 8.5, CancellationToken.None);

        _uow.Verify(x => x.Reviews.AddAsync(
            It.Is<Review>(r => r.UserId == 5 && r.MovieId == 20 && r.Status == ReviewStatus.Pending && r.Rating == 8.5),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByMovieIdAsync_Empty_ReturnsEmpty()
    {
        _uow.Setup(x => x.Reviews.GetByMovieIdAsync(99, ReviewStatus.Approved, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review>());

        var result = await _sut.GetByMovieIdAsync(99, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByMovieIdAsync_MapsAllFields()
    {
        var created = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var reviews = new List<Review>
        {
            new()
            {
                Id = 3,
                MovieId = 7,
                UserId = 2,
                Text = "Неплохо",
                Rating = 6.5,
                Status = ReviewStatus.Approved,
                CreatedAt = created,
                Movie = null!
            }
        };
        _uow.Setup(x => x.Reviews.GetByMovieIdAsync(7, ReviewStatus.Approved, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);

        var result = await _sut.GetByMovieIdAsync(7, CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(3);
        result[0].MovieId.Should().Be(7);
        result[0].UserId.Should().Be(2);
        result[0].Rating.Should().Be(6.5);
        result[0].CreatedAt.Should().Be(created);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtUtc()
    {
        Review? captured = null;
        _uow.Setup(x => x.Reviews.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Callback<Review, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var before = DateTime.UtcNow.AddSeconds(-2);
        await _sut.CreateAsync(1, 1, "ok", 7, CancellationToken.None);
        var after = DateTime.UtcNow.AddSeconds(2);

        captured.Should().NotBeNull();
        captured!.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        captured.Status.Should().Be(ReviewStatus.Pending);
    }

    [Fact]
    public async Task CreateAsync_ZeroRating_StillAdds()
    {
        _uow.Setup(x => x.Reviews.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sut.CreateAsync(1, 1, "meh", 0, CancellationToken.None);

        _uow.Verify(x => x.Reviews.AddAsync(
            It.Is<Review>(r => r.Rating == 0 && r.Text == "meh"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}