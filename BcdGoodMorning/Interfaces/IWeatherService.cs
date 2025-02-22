using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string zipcode);
}