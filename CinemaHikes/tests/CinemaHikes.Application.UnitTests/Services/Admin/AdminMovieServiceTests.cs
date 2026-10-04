using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CinemaHikes.Application.UnitTests.Services.Admin;

public class AdminMovieServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly AdminMovieService _sut;

    public AdminMovieServiceTests() => _sut = new AdminMovieService(_uow.Object);

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesAndReturnsId()
    {
        var dto = new CreateMovieDto("Title", "Ua", "Eng", "Desc", "Dir", 2022, "poster", 8.5, new[] { 1 });
        var genre = new Genre { Id = 1, Name = "Action" };

        _uow.Setup(x => x.Genres.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(genre);
        _uow.Setup(x => x.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Callback<Movie, CancellationToken>((m, _) => m.Id = 42)
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var id = await _sut.CreateAsync(dto, CancellationToken.None);

        id.Should().Be(42);
        _uow.Verify(x => x.Movies.AddAsync(It.Is<Movie>(m => m.RuTitle == "Title"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_GenreNotFound_Throws()
    {
        var dto = new CreateMovieDto("T", "U", "E", "D", "Dir", 2020, "p", 7, new[] { 999 });
        _uow.Setup(x => x.Genres.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Genre?)null);

        var act = () => _sut.CreateAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Genre 999 not found*");
    }

    [Fact]
    public async Task DeleteAsync_MovieNotFound_Throws()
    {
        _uow.Setup(x => x.Movies.GetByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync((Movie?)null);

        var act = () => _sut.DeleteAsync(100, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Movie 100 not found*");
    }
}