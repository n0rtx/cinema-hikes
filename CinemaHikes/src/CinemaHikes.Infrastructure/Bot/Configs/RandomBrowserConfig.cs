using CinemaHikes.Domain.Interfaces.Bot;
using CinemaHikes.Infrastructure.Bot.Providers;
using Microsoft.Playwright;

namespace CinemaHikes.Infrastructure.Bot.Configs;

public class RandomBrowserConfig : IBrowserConfig
{
    public BrowserTypeLaunchOptions CreateLaunchOptions()
    {
        return new BrowserTypeLaunchOptions()
        {
            Headless = true,
            Args =
            [
                "--disable-blink-features=AutomationControlled",
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-infobars",
                "--disable-features=IsolateOrigins,site-per-process,SameSiteByDefaultCookies,CookiesWithoutSameSiteMustBeSecure",
                "--allow-third-party-cookies"
            ]
        };
    }

    public BrowserNewContextOptions CreateContextOptions()
    {
        var profile = BrowserProfileProvider.GetRandomProfile();

        return new BrowserNewContextOptions
        {
            UserAgent = profile.UserAgent,
            ViewportSize = profile.Viewport,
            Locale = profile.Locale,
            TimezoneId = profile.TimezoneId,
            ExtraHTTPHeaders = profile.ToExtraHttpHeaders(),
            BypassCSP = true,
            IgnoreHTTPSErrors = true
        };
    }
}