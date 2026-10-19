using System;
using System.Threading.Tasks;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces.Bot.Parsers;
using Microsoft.Playwright;

namespace CinemaHikes.Infrastructure.Bot.Parsers.Rezka;

public class RezkaMovieUrlParser : IMovieUrlParser
{
    public async Task<string> GetMovieUrlAsync(IPage page, VideoQuality videoQuality)
    {
        string? foundUrl = null;
        
        page.Request += (_, request) =>
        {
            var url = request.Url;
            if (url.Contains(".mp4") && !url.Contains("preview"))
            {
                foundUrl = url;
            }
        };

        int choiceId = GetVideoIdByQuality(videoQuality);
        await ProceedQualityElementAsync(page, choiceId);
        
        int elapsed = 0;
        while (string.IsNullOrEmpty(foundUrl) && elapsed < 10000)
        {
            await Task.Delay(250);
            elapsed += 250;
        }

        if (string.IsNullOrEmpty(foundUrl))
            throw new Exception("Playwright не смог перехватить mp4 поток в сетевых запросах.");

        return CleanUrl(foundUrl);
    }

    private int GetVideoIdByQuality(VideoQuality videoQuality)
    {
        return videoQuality switch
        {
            VideoQuality.Low => 1,
            VideoQuality.Medium => 2,
            VideoQuality.High => 3,
            VideoQuality.FullHd => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(videoQuality), "No such video quality.")
        };
    }

    private async Task ProceedQualityElementAsync(IPage page, int choiceId)
    {
        var qualityElement = page.Locator($"pjsdiv[f2id='{choiceId}']");

        await qualityElement.WaitForAsync(new LocatorWaitForOptions()
        {
            State = WaitForSelectorState.Attached,
            Timeout = 5000
        });

        await qualityElement.ClickAsync(new LocatorClickOptions() { Force = true });
        
        await Task.Delay(1500);

        try 
        {
            await page.Locator("#oframecdnplayer, video").First.ClickAsync(new LocatorClickOptions { Force = true });
        } 
        catch { }
    }

    private string CleanUrl(string url)
    {
        int lastIndex = url.Length;

        for (int i = 3; i < url.Length; i++)
        {
            if (url[i] == '4' && url[i - 1] == 'p' && url[i - 2] == 'm' && url[i - 3] == '.')
            {
                lastIndex = i + 1;
                break;
            }
        }

        return url[..lastIndex];
    }
}