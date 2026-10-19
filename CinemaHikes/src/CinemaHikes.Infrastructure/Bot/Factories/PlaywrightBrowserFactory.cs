using CinemaHikes.Domain.Interfaces.Bot;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace CinemaHikes.Infrastructure.Bot.Factories;

public class PlaywrightBrowserFactory : IBrowserFactory
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    
    public async Task<IPage> CreatePageAsync(IBrowserConfig browserConfig)
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(browserConfig.CreateLaunchOptions());
        _context = await _browser.NewContextAsync(browserConfig.CreateContextOptions());
        
        return await _context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
            await _browser.DisposeAsync();
        
        _playwright?.Dispose();
    }
}