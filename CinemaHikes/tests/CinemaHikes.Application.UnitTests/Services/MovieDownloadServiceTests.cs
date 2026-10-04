using CinemaHikes.Application.Services;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using CinemaHikes.Domain.Interfaces.Bot;
using FluentAssertions;
using Moq;

namespace CinemaHikes.Application.UnitTests.Services;

public class MovieDownloadServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMovieParserFacade> _parser = new();
    private readonly Mock<IVideoSizeChecker> _sizeChecker = new();
    private readonly MovieDownloadService _sut;

    public MovieDownloadServiceTests()
    {
        _sut = new MovieDownloadService(_uow.Object, _parser.Object, _sizeChecker.Object);
    }

    [Fact]
    public async Task GetDownloadAsync_WhenCachedLinkExists_ReturnsIt()
    {
        var cached = new MovieLink
        {
            Url = "https://cdn.example/movie.mp4",
            MovieId = 1,
            VideoQuality = VideoQuality.High,
            Movie = null!,
            TranslationStudio = null!,
            UpdatedAt = DateTime.UtcNow
        };
        _uow.Setup(x => x.MovieLinks.GetByMovieAndQualityAsync(1, VideoQuality.High, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);
        _sizeChecker.Setup(x => x.IsWithinTelegramLimitAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GetDownloadAsync(1, VideoQuality.High, CancellationToken.None);

        result.Url.Should().Be("https://cdn.example/movie.mp4");
        _parser.Verify(x => x.GetMovieSrcAsync(It.IsAny<string>(), It.IsAny<VideoQuality>()), Times.Never);
    }

    [Fact]
    public async Task GetDownloadAsync_NoActiveSources_Throws()
    {
        _uow.Setup(x => x.MovieLinks.GetByMovieAndQualityAsync(1, VideoQuality.FullHd, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MovieLink?)null);
        _uow.Setup(x => x.VideoSources.GetByMovieIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<VideoSource>());

        var act = () => _sut.GetDownloadAsync(1, VideoQuality.FullHd, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No active sources*");
    }

    [Fact]
    public async Task GetDownloadAsync_ParsesFromSource_AndSavesLink()
    {
        _uow.Setup(x => x.MovieLinks.GetByMovieAndQualityAsync(2, VideoQuality.Medium, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MovieLink?)null);

        var sources = new List<VideoSource>
        {
            new()
            {
                Id = 1,
                MovieId = 2,
                PageUrl = "https://rezka.ag/film",
                Priority = 1,
                Status = SourceStatus.Active,
                ProviderName = "Rezka",
                Movie = null!
            }
        };
        _uow.Setup(x => x.VideoSources.GetByMovieIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sources);

        _parser.Setup(x => x.GetMovieSrcAsync("https://rezka.ag/film", VideoQuality.Medium))
            .ReturnsAsync("https://stream.example/480.mp4");

        _uow.Setup(x => x.TranslationStudios.GetByNameAsync("Default", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TranslationStudio { Id = 1, Name = "Default" });
        _uow.Setup(x => x.MovieLinks.UpsertAsync(It.IsAny<MovieLink>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _sizeChecker.Setup(x => x.IsWithinTelegramLimitAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GetDownloadAsync(2, VideoQuality.Medium, CancellationToken.None);

        result.Url.Should().Be("https://stream.example/480.mp4");
        _uow.Verify(x => x.MovieLinks.UpsertAsync(It.IsAny<MovieLink>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDownloadAsync_AllParsersFail_Throws()
    {
        _uow.Setup(x => x.MovieLinks.GetByMovieAndQualityAsync(3, VideoQuality.High, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MovieLink?)null);

        var sources = new List<VideoSource>
        {
            new()
            {
                PageUrl = "https://bad.source",
                Priority = 1,
                Status = SourceStatus.Active,
                ProviderName = "Bad",
                MovieId = 0,
                Movie = null!
            }
        };
        _uow.Setup(x => x.VideoSources.GetByMovieIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sources);

        _parser.Setup(x => x.GetMovieSrcAsync(It.IsAny<string>(), It.IsAny<VideoQuality>()))
            .ThrowsAsync(new Exception("parse fail"));

        var act = () => _sut.GetDownloadAsync(3, VideoQuality.High, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Could not resolve stream*");
    }
}