using BcdGoodMorning.Services;

namespace BcdGoodMorning.Models;

public class DailyEmailModel
{
    public string Location { get; set; }
    public string Salutation { get; set; }
    public WordOfTheDay WordOfTheDay { get; set; }
    public WikipediaData WikipediaData { get; set; }
    public List<NewsArticle> NewsArticles { get; set; }
    public DailyWeather TodayWeather { get; set; }
    public List<WeatherAlert> WeatherAlerts { get; set; }
    public List<DailyWeather> Forecast { get; set; }
    public Artwork ArtImage { get; set; }
}