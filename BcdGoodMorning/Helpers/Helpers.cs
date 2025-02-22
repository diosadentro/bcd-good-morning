using Microsoft.Playwright;

namespace BcdGoodMorning;

public static class Helpers
{
    public static async Task<Tuple<string, string>> GetSiteInfo(string url)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync(new BrowserNewPageOptions()
        {
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            ExtraHTTPHeaders = new []
            {
                new KeyValuePair<string, string>("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"),
                new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.5"),
                new KeyValuePair<string, string>("Referrer", "https://www.google.com"),
                new KeyValuePair<string, string>("Connection", "keep-alive")
            }
        });
        await page.GotoAsync(url, new PageGotoOptions { Timeout = 60000 });
        var innerHtml = await page.InnerHTMLAsync("html");
        return new Tuple<string, string>(page.Url, $"<html>{innerHtml}</html>");
    }
}