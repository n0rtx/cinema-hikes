using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Bot;
using CinemaHikes.Domain.Interfaces.Bot.Parsers;
using CinemaHikes.Infrastructure.Bot.Configs;
using CinemaHikes.Infrastructure.Bot.Factories;
using CinemaHikes.Infrastructure.Bot.Parsers.Rezka;

namespace CinemaHikes.Infrastructure.Bot.Facades;

public class RezkaMovieParserFacade : IMovieParserFacade
{
    public async Task<string> GetMovieSrcAsync(string pageUrl, VideoQuality videoQuality)
    {
        IBrowserConfig kievBrowserConfig = new RandomBrowserConfig();
        
        IBrowserFactory playwrightFactory = new PlaywrightBrowserFactory();
        var page = await playwrightFactory.CreatePageAsync(kievBrowserConfig);

        IPageConfig pageConfig = new RezkaPageConfig();
        await pageConfig.InitPageAsync(page, pageUrl);

        IPageElementParser gearIconParser = new RezkaGearIconParser();
        await gearIconParser.ParseElementAsync(page);

        IPageElementParser qualityMenuParser = new RezkaVideoQualityMenuParser();
        await qualityMenuParser.ParseElementAsync(page);

        IMovieUrlParser movieUrlParser = new RezkaMovieUrlParser();
        string url = await movieUrlParser.GetMovieUrlAsync(page, videoQuality);
        
        return url;
    }
}