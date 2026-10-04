using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Specifications;
using FluentAssertions;
using Moq;

namespace CinemaHikes.Application.UnitTests.Services;

public class GenreServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly GenreService _sut;

    public GenreServiceTests() => _sut = new GenreService(_uow.Object);

    [Fact]
    public async Task GetAllAsync_ReturnsAllGenres()
    {
        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Боевик" },
            new() { Id = 2, Name = "Комедия" }
        };
        _uow.Setup(x => x.Genres.ListAsync(It.IsAny<EmptySpecification<Genre>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(genres);

        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Боевик");
        result[1].Id.Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        _uow.Setup(x => x.Genres.ListAsync(It.IsAny<EmptySpecification<Genre>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre>());

        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }
}