using CinemaHikes.Domain.Interfaces.Bot;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace CinemaHikes.Infrastructure.Bot.Configs;

public class RezkaPageConfig : IPageConfig
{
    public async Task InitPageAsync(IPage page, string pageUrl)
    {
        await page.GotoAsync(pageUrl, new PageGotoOptions()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000
        });

        bool isAnubis = await page.Locator("text=Anubis").CountAsync() > 0 || await page.TitleAsync() == "О нет!";
        
        if (isAnubis)
        {
            await page.WaitForSelectorAsync("#oframecdnplayer, video", new PageWaitForSelectorOptions 
            { 
                Timeout = 25000 
            });
            await Task.Delay(2000);
        }
        else
        {
            await Task.Delay(3500);
        }
    }
}