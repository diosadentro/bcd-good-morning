using System.Web;
using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BcdGoodMorning.Services;

public class WeatherService(IOptions<ConfigurationOptions> _options, ILogger<WeatherService> _logger) : IWeatherService
{
    private HttpClient _httpClient = new HttpClient();
    
    public async Task<WeatherData> GetWeatherAsync(string zipcode)
    {
        try
        {
            var latLongData = await GetLatLongFromZip(zipcode);
            var queryParams = new Dictionary<string, string>
            {
                { "lat", latLongData.Lat },
                { "lon", latLongData.Lon },
                { "exclude", "hourly" },
                { "appid", _options.Value.WeatherApiKey },
                { "units", "imperial" }
            };

            var uriBuilder = new UriBuilder("https://api.openweathermap.org/data/3.0/onecall");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();
            var url = uriBuilder.ToString();

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Convert the json  to WeatherData object
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var weatherData = JsonConvert.DeserializeObject<WeatherData>(content, settings);

            return weatherData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather data");
            return null;
        }
    }

    private async Task<NominatimResult> GetLatLongFromZip(string zipCode)
    {
        string url = $"https://nominatim.openstreetmap.org/search?postalcode={zipCode}&country=US&format=json";

        using HttpClient client = new();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("C# App"); // Required for Nominatim

        string response = await client.GetStringAsync(url);
        var results = JsonConvert.DeserializeObject<NominatimResult[]>(response);

        return results.FirstOrDefault();
    }
}