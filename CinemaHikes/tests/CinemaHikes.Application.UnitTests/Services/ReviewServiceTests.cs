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
}