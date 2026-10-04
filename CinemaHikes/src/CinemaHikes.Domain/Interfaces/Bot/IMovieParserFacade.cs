using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Interfaces.Bot;

public interface IMovieParserFacade
{
    public Task<string> GetMovieSrcAsync(string pageUrl, VideoQuality videoQuality);
}