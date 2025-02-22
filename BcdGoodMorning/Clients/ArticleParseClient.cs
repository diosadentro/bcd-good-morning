using System.Text;
using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BcdGoodMorning.Clients;

public class ArticleParseClient : IArticleParseClient
{
    private readonly ConfigurationOptions _options;
    private readonly HttpClient _client;
    public ArticleParseClient(IOptions<ConfigurationOptions> options)
    {
        _options = options.Value;
        _client = new HttpClient();
        _client.BaseAddress = new Uri(_options.ArticleParseUrl);
    }
    
    public async Task<NewsArticleParse> GetArticleSummaryAsync(string url)
    {
        var response = await _client.PostAsync("/parse", 
            new StringContent(JsonConvert.SerializeObject(new { url }), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var article = JsonConvert.DeserializeObject<NewsArticleParse>(content);
        return article;
    }
    
    
}