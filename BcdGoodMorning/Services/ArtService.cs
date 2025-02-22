using BcdGoodMorning.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BcdGoodMorning.Services;

public class ArtService : IArtService
{
    private readonly ILogger<ArtService> _logger;
    private readonly ConfigurationOptions _config;
    public ArtService(ILogger<ArtService> logger, IOptions<ConfigurationOptions> config)
    {
        _logger = logger;
        _config = config.Value;
    }
    
    public async Task<Artwork> GetArtOfTheDay()
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("AIC-User-Agent", $"personal-digest-email ({_config.ReportEmailAddress})");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
                
                var queryObject = new
                {
                    query = new
                    {
                        match = new
                        {
                            term_titles = "painting"
                        }
                    }
                };
                
                var response = await client.PostAsync("https://api.artic.edu/api/v1/artworks?page=1&limit=1", new StringContent(JsonConvert.SerializeObject(queryObject)));
                response.EnsureSuccessStatusCode();
                var pages = await response.Content.ReadAsStringAsync();
                var pageInfo = JsonConvert.DeserializeObject<ArtModel>(pages);

                if (pageInfo?.Pagination?.TotalPages == null)
                {
                    _logger.LogWarning("Invalid pagination data received");
                    return null;
                }

                var totalPages = pageInfo.Pagination.TotalPages;
                var randomPage = new Random().Next(1, totalPages + 1);

                var art = await client.PostAsync($"https://api.artic.edu/api/v1/artworks?page={randomPage}&limit=1", new StringContent(JsonConvert.SerializeObject(queryObject)));
                art.EnsureSuccessStatusCode();
                var artContent = await art.Content.ReadAsStringAsync();
                var artInfo = JsonConvert.DeserializeObject<ArtModel>(artContent);
            
                return artInfo.Data[0];
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting art of the day");
            throw;
        }
    }
}