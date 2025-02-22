using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BcdGoodMorning.Services;

public class WikipediaService : IWikipediaService
{
    private readonly ILogger<WikipediaService> _logger;
    
    public WikipediaService(ILogger<WikipediaService> logger)
    {
        _logger = logger;
    }
    
    public async Task<WikipediaData> GetRandomWikipediaArticleAsync()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync("https://en.wikipedia.org/api/rest_v1/page/random/summary");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var wikiData = JsonConvert.DeserializeObject<WikipediaData>(content, settings);
            _logger.LogInformation("Random Wikipedia Article Retrieved: {title}", wikiData.Title);
            return wikiData;
        }
    }
}