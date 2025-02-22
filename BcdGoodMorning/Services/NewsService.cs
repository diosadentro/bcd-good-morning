using System.Text.RegularExpressions;
using System.Xml;
using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using FuzzySharp;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using HttpClient = System.Net.Http.HttpClient;

namespace BcdGoodMorning.Services;

public class NewsService : INewsService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly IOptions<ConfigurationOptions> _options;
    private readonly ILogger<NewsService> _logger;
    private readonly ILLMClient _llmClient;
    private readonly IArticleParseClient _parseClient;

    public NewsService(IOptions<ConfigurationOptions> options,
        ILogger<NewsService> logger,
        ILLMClient llmClient,
        IArticleParseClient parseClient)
    {
        _options = options;
        _logger = logger;
        _llmClient = llmClient;
        _parseClient = parseClient;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
    }
    
    public async Task<List<NewsArticle>> GetNewsAsync()
    {
        var newsArticles = new List<NewsArticle>();
        foreach (var feed in _options.Value.RssNewsFeedUrls.OrderBy(x => x.Weight))
        {
            var feedContent = await GetRssFeedAsync(feed);
            // Sleep 1 second to avoid rate limiting on same url domains
            Thread.Sleep(1000);

            if (feed.MaxItems > 0)
            {
                var limitedFeedContent = feedContent.OrderByDescending(x => x.PubDate).Take(feed.MaxItems).ToList();
                newsArticles.AddRange(limitedFeedContent);
            }
            else
            {
                newsArticles.AddRange(feedContent);
            }
        }

        // Get unqiue articles
        newsArticles = GetUniqueArticles(newsArticles);
        
        // Get Summary for articles
        foreach (var newsArticle in newsArticles)
        {
            try
            {
                // Get Perm URLs and raw HTML. We don't use the Raw HTML atm, but nice to have
                var siteInfo = await Helpers.GetSiteInfo(newsArticle.Link);
                newsArticle.Link = siteInfo.Item1;
                newsArticle.RawHtml = siteInfo.Item2;
                
                // Send through to parser service that uses postlight parser to get article details
                await GetArticleDetails(newsArticle);
                if(newsArticle.ParseData?.LeadImageUrl != null)
                {
                    newsArticle.Image = newsArticle.ParseData.LeadImageUrl;
                }
                else
                {
                    // Backup attempt to get article image
                    newsArticle.Image = await GetArticleImage(newsArticle);
                }
                
                // LLM Get summary
                await GetNewsSummaryAsync(newsArticle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news summary for article {newsArticle}", newsArticle.Title);
                newsArticle.Summary = null;
                newsArticle.Link = null;
            }
        }
        
        // Remove articles without a summary or link
        newsArticles = newsArticles.Where(x => !string.IsNullOrEmpty(x.Summary) && !string.IsNullOrEmpty(x.Link)).ToList();
        
        return newsArticles;
    }
    private List<NewsArticle> GetUniqueArticles(List<NewsArticle> titles, int threshold = 85)
    {
        List<NewsArticle> distinctTitles = new List<NewsArticle>();

        foreach (var title in titles)
        {
            bool isDuplicate = distinctTitles.Any(existingTitle =>
                Fuzz.Ratio(title.Title, existingTitle.Title) >= threshold);

            if (!isDuplicate)
            {
                distinctTitles.Add(title);
            }
        }

        return distinctTitles;
    }
    private async Task<List<NewsArticle>> GetRssFeedAsync(NewSource source)
    {
        var newsItems = new List<NewsArticle>();
        // Get the RSS Feed
        try
        {
            var response = await _httpClient.GetAsync(source.Url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();

            using (var reader = XmlReader.Create(content))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
                    {
                        var newsArticle = new NewsArticle();
                        newsArticle.Weight = source.Weight;
                        while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "item"))
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                var elementName = reader.Name;
                                reader.Read(); // Move to the text node
                                switch (elementName)
                                {
                                    case "title":
                                        newsArticle.Title = reader.Value;
                                        break;
                                    case "link":
                                        newsArticle.Link = reader.Value;
                                        break;
                                    case "guid":
                                        newsArticle.Guid = reader.Value;
                                        break;
                                    case "pubDate":
                                        newsArticle.PubDate = DateTime.Parse(reader.Value);
                                        break;
                                    case "description":
                                        newsArticle.Description = reader.Value;
                                        break;
                                    case "source":
                                        newsArticle.Source = reader.Value;
                                        break;
                                }
                            }
                        }

                        if (newsArticle.PubDate > DateTime.Now.AddDays(-1))
                        {
                            newsItems.Add(newsArticle);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RSS feed from {feedUrl}", source.Url);
        }
        return newsItems;
    }
    private async Task GetArticleDetails(NewsArticle article)
    {
        try
        {
            var details = await _parseClient.GetArticleSummaryAsync(article.Link);
            if (!string.IsNullOrWhiteSpace(details?.Content))
            {
                // Get rid of HTML content to keep token size smaller
                details.Content = Regex.Replace(details.Content, "<.*?>", string.Empty);
            }

            article.ParseData = details;
        }
        catch (Exception e)
        {
            // Do nothing
        }
    }
    private async Task<string> GetArticleImage(NewsArticle data)
    {
        // If we were unable to get the raw HTML content, try to get it a different way
        if (string.IsNullOrWhiteSpace(data.RawHtml))
        {
            try
            {
                // Send HTTP GET request to the URL
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Accept.ParseAdd(
                        "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://www.google.com/");
                    client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                    data.RawHtml = await client.GetStringAsync(data.Link);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting HTML content for article {newsArticle}. Trying fallback method",
                    data.Link);
            }
        }

        try
        {
            if (!string.IsNullOrEmpty(data.RawHtml))
            {
                // Parse the HTML content
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(data.RawHtml);

                // Find all <meta> tags with property attribute starting with 'og:'
                var ogTag = htmlDoc.DocumentNode
                    .Descendants("meta")
                    .FirstOrDefault(node => node.GetAttributeValue("property", "").StartsWith("og:image"));

                if (ogTag != null)
                {
                    return ogTag.GetAttributeValue("content", "");
                }
                
                // Fallback to twitter:image if OG image isn't available
                var twitterImage = htmlDoc.DocumentNode.Descendants("meta")
                    .FirstOrDefault(node => node.GetAttributeValue("name", "") == "twitter:image")
                    ?.GetAttributeValue("content", "");

                if (!string.IsNullOrEmpty(twitterImage))
                {
                    return twitterImage;  // If Twitter image is found, return it
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Open Graph metadata for article {newsArticle}", data.Title);
        }
        return "";
    }
    private async Task GetNewsSummaryAsync(NewsArticle article)
    {
        try
        {
            // If the article parser was able to pull article details, then use them to generate a summary
            if (article.ParseData?.Content != null)
            {
                var system = "You are a summarization agent. " +
                             "Your job is to take the provided content and generate a summary. " +
                             "The summary should be concise and only include the most important information. " +
                             "Do not include any opinions or additional information. " +
                             "Avoid headers or the website URL. " +
                             "Avoid repetitive information. " +
                             "Use exact numbers from the article when including data. " +
                             "If relevant, include bullets or lists for information. " +
                             "Use HTML tags such as <p> for better readability.";
                
                var request = new LLMRequest()
                {
                    SystemPrompt = system,
                    UserPrompt = $"Please summarize the following article: {article.ParseData.Content}"
                };
            
                
                article.Summary = await _llmClient.GetResponseAsync(request);
            }
            // If unable to do so, let's let groq try to pull the article.
            // This method is not as reliable for summaries but gets around some blocks
            else
            {
                var prompt =
                    "You are a news bot. " +
                    "Your job is to pull out important key facts from an article " +
                    "Do not use opinions. " +
                    "The facts should only include information from the article. " +
                    "Avoid headers or the website URL. " +
                    "Avoid repetitive information. " +
                    "Use exact numbers from the article when including data. " +
                    "If relevant, include bullets or lists for information. " +
                    "Use HTML tags such a <p>, <ol>, <li>, etc for better readability.";

                var request = new LLMRequest()
                {
                    SystemPrompt = prompt,
                    UserPrompt = $"The article is located at {article.Link}"
                };

                article.OriginalSummary = await _llmClient.GetResponseAsync(request);

                Thread.Sleep(3000);

                var system =
                    "You are an editor and a fact checker for a news organization. " +
                    "Review the facts provided and correct them if they are incorrect. " +
                    "Be sure to review all numbers and dates for accuracy. " +
                    "Correct them if they are wrong by replacing the original values with the correct values, " +
                    "do not include both correct and incorrect values. " +
                    "Remove facts that you are unable to verify. " +
                    "Return the information in the same format as the input, preserving the HTML content. " +
                    "Do not provide any output other than the facts." +
                    "Do not include sources or citations, do not include information about what you corrected.";
                
                prompt = $"The facts are: {article.OriginalSummary}. The article is located at {article.Link}.";

                request = new LLMRequest()
                {
                    SystemPrompt = system,
                    UserPrompt = prompt
                };

                article.Summary = await _llmClient.GetResponseAsync(request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting news summary for article {newsArticle}", article.Link);
        }
    }
}