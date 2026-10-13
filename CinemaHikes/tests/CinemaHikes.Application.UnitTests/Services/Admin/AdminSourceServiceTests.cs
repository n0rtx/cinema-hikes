using CinemaHikes.Application.Dtos.Admin;
using CinemaHikes.Application.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CinemaHikes.Application.UnitTests.Services.Admin;

public class AdminSourceServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly AdminSourceService _sut;

    public AdminSourceServiceTests() => _sut = new AdminSourceService(_uow.Object);

    [Fact]
    public async Task UpdateStatusAsync_SourceNotFound_Throws()
    {
        _uow.Setup(x => x.VideoSources.GetByIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoSource?)null);

        var act = () => _sut.UpdateStatusAsync(50, SourceStatus.Dead, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Source 50 not found*");
    }

    [Fact]
    public async Task UpdateStatusAsync_Existing_UpdatesAndSaves()
    {
        var source = new VideoSource
        {
            Id = 7,
            MovieId = 1,
            ProviderName = "Rezka",
            PageUrl = "https://example.com",
            Priority = 1,
            Status = SourceStatus.Active,
            Movie = null!
        };
        _uow.Setup(x => x.VideoSources.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sut.UpdateStatusAsync(7, SourceStatus.Dead, CancellationToken.None);

        source.Status.Should().Be(SourceStatus.Dead);
        _uow.Verify(x => x.VideoSources.Update(source), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByMovieIdAsync_ReturnsMappedList()
    {
        var list = new List<VideoSource>
        {
            new()
            {
                Id = 1,
                MovieId = 10,
                ProviderName = "Rezka",
                PageUrl = "https://rezka.ag/1",
                Priority = 1,
                Status = SourceStatus.Active,
                Movie = null!
            }
        };
        _uow.Setup(x => x.VideoSources.GetByMovieIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _sut.GetByMovieIdAsync(10, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ProviderName.Should().Be("Rezka");
        result[0].Status.Should().Be(SourceStatus.Active);
    }

    [Fact]
    public async Task GetByMovieIdAsync_Empty_ReturnsEmpty()
    {
        _uow.Setup(x => x.VideoSources.GetByMovieIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<VideoSource>());

        var result = await _sut.GetByMovieIdAsync(99, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddSourceAsync_AddsActiveAndSaves()
    {
        _uow.Setup(x => x.VideoSources.AddAsync(It.IsAny<VideoSource>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        await _sut.AddSourceAsync(new AdminSourceDto(1, "Rezka", "https://rezka.ag/film", 2), CancellationToken.None);

        _uow.Verify(x => x.VideoSources.AddAsync(
            It.Is<VideoSource>(s =>
                s.MovieId == 1 &&
                s.ProviderName == "Rezka" &&
                s.PageUrl == "https://rezka.ag/film" &&
                s.Priority == 2 &&
                s.Status == SourceStatus.Active),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}