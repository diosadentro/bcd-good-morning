using System.Text.RegularExpressions;
using System.Xml.Linq;
using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using HtmlAgilityPack;

namespace BcdGoodMorning.Services;

public class WordOfTheDayService : IWordOfTheDayService
{
    private readonly ILogger<WordOfTheDayService> _logger;
    
    public WordOfTheDayService(ILogger<WordOfTheDayService> logger)
    {
        _logger = logger;
    }

    private string CleanHtml(string description)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(description);
        foreach (var fontTag in doc.DocumentNode.SelectNodes("//font") ?? new HtmlNodeCollection(null))
        {
            fontTag.Attributes.Remove();
        }

        // Get the cleaned HTML
        string cleanedHtml = doc.DocumentNode.OuterHtml;
        
        // Remove additional tags
        // Regex to remove only the HTML tags containing "Merriam-Webster's Word of the Day for *"
        string pattern = @"<font>\n*?\s*?<p>\n*?\s*?<strong>\n*?\s*?<font>Merriam-Webster's Word of the Day for .*<\/font>\n*?\s*?<\/strong>\n*?\s*?<\/p>\n*?\s*?";

        string didYouKnowPattern =
            @"<p>\n*?\s*?<strong>Did you know\?<\/strong><br>\n*?\s*?<p>.*?<\/p>\n*?\s*?<br><br>\n*?\s*?";
        // Replace the matched tags with an empty string
        cleanedHtml = Regex.Replace(cleanedHtml, pattern, "", RegexOptions.IgnoreCase).Trim();
        
        cleanedHtml = Regex.Replace(cleanedHtml, didYouKnowPattern, "", RegexOptions.IgnoreCase).Trim();
        
        return cleanedHtml;
    }
    
    public async Task<WordOfTheDay> GetWordOfTheDayAsync()
    {
        var url = "https://www.merriam-webster.com/wotd/feed/rss2";
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var doc = XDocument.Parse(content);

            var items = doc.Descendants("item");
            foreach (var item in items)
            {
                var title = item?.Element("title")?.Value;
                var description = item?.Element("description")?.Value;
                var pubDate = item?.Element("pubDate")?.Value;
                var link = item?.Element("link")?.Value;
                var pronunciation = item?.Element("enclosure")?.Attribute("url")?.Value;
                
                if(pubDate != null)
                {
                    var dateRemovedTimezone = pubDate.Substring(0, pubDate.LastIndexOf("-"));
                    var date = DateTime.Parse(dateRemovedTimezone);

                    if(date.Date == DateTime.Today.Date)
                    {
                        _logger.LogInformation("Word of the Day Retrieved: {title}", title);
                        return new WordOfTheDay()
                        {
                            Definition = CleanHtml(description),
                            Word = title,
                            Pronunciation = pronunciation,
                            Url = link
                        };
                    }
                }
            }
        }

        return null;
    }
}