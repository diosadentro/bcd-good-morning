using Newtonsoft.Json;

namespace BcdGoodMorning.Models;


public class WeatherData
{
    [JsonProperty("lat")] public double Latitude { get; set; }
    [JsonProperty("lon")] public double Longitude { get; set; }
    [JsonProperty("timezone")] public string Timezone { get; set; }
    [JsonProperty("timezone_offset")] public int TimezoneOffset { get; set; }
    [JsonProperty("current")] public CurrentWeather Current { get; set; }
    [JsonProperty("minutely")] public List<MinutelyWeather> Minutely { get; set; }
    [JsonProperty("hourly")] public List<HourlyWeather> Hourly { get; set; }
    [JsonProperty("daily")] public List<DailyWeather> Daily { get; set; }
    [JsonProperty("alerts")] public List<WeatherAlert> Alerts { get; set; }
}

public class CurrentWeather
{
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("dt")] public DateTime DateTime { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("sunrise")] public DateTime Sunrise { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("sunset")] public DateTime Sunset { get; set; }
    [JsonProperty("temp")] public double Temperature { get; set; }
    [JsonProperty("feels_like")] public double FeelsLike { get; set; }
    [JsonProperty("pressure")] public int Pressure { get; set; }
    [JsonProperty("humidity")] public int Humidity { get; set; }
    [JsonProperty("dew_point")] public double DewPoint { get; set; }
    [JsonProperty("uvi")] public double UVIndex { get; set; }
    [JsonProperty("clouds")] public int Clouds { get; set; }
    [JsonProperty("visibility")] public int Visibility { get; set; }
    [JsonProperty("wind_speed")] public double WindSpeed { get; set; }
    [JsonProperty("wind_deg")] public int WindDirection { get; set; }
    [JsonProperty("wind_gust")] public double WindGust { get; set; }
    [JsonProperty("weather")] public List<WeatherCondition> Weather { get; set; }
}

public class WeatherCondition
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("main")] public string Main { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("icon")] public string Icon { get; set; }

    public string DisplayDescription
    {
        get
        {
            return Description.Substring(0,1).ToUpper() + Description.Substring(1);
        }
    }
}

public class MinutelyWeather
{
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("dt")] public DateTime DateTime { get; set; }
    [JsonProperty("precipitation")] public double Precipitation { get; set; }
}

public class HourlyWeather : CurrentWeather
{
    [JsonProperty("pop")] public double ProbabilityOfPrecipitation { get; set; }
}

public class DailyWeather
{
    [JsonConverter(typeof(UnixTimestampConverter))] [JsonProperty("dt")] public DateTime DateTime { get; set; }
    [JsonConverter(typeof(UnixTimestampConverter))] [JsonProperty("sunrise")] public DateTime Sunrise { get; set; }
    [JsonConverter(typeof(UnixTimestampConverter))] [JsonProperty("sunset")] public DateTime Sunset { get; set; }
    [JsonConverter(typeof(UnixTimestampConverter))] [JsonProperty("moonrise")] public DateTime Moonrise { get; set; }
    [JsonConverter(typeof(UnixTimestampConverter))] [JsonProperty("moonset")] public DateTime Moonset { get; set; }
    [JsonProperty("moon_phase")] public double MoonPhase { get; set; }
    [JsonProperty("summary")] public string Summary { get; set; }
    [JsonProperty("temp")] public TemperatureDetails Temperature { get; set; }
    [JsonProperty("feels_like")] public FeelsLikeDetails FeelsLike { get; set; }
    [JsonProperty("pressure")] public int Pressure { get; set; }
    [JsonProperty("humidity")] public int Humidity { get; set; }
    [JsonProperty("dew_point")] public double DewPoint { get; set; }
    [JsonProperty("wind_speed")] public double WindSpeed { get; set; }
    [JsonProperty("wind_deg")] public int WindDirection { get; set; }

    public string WindCardinalDirection
    {
        get
        {
            return GetWindDirection(WindDirection);
        }
    }

    [JsonProperty("wind_gust")] public double WindGust { get; set; }
    [JsonProperty("weather")] public List<WeatherCondition> Weather { get; set; }
    [JsonProperty("clouds")] public int Clouds { get; set; }
    [JsonProperty("pop")] public double ProbabilityOfPrecipitation { get; set; }
    [JsonProperty("rain")] public double? Rain { get; set; }
    [JsonProperty("uvi")] public double UVIndex { get; set; }
    
    public string GetWindDirection(double degrees)
    {
        if (degrees >= 0 && degrees < 22.5) return "N";
        else if (degrees >= 22.5 && degrees < 67.5) return "NE";
        else if (degrees >= 67.5 && degrees < 112.5) return "E";
        else if (degrees >= 112.5 && degrees < 157.5) return "SE";
        else if (degrees >= 157.5 && degrees < 202.5) return "S";
        else if (degrees >= 202.5 && degrees < 247.5) return "SW";
        else if (degrees >= 247.5 && degrees < 292.5) return "W";
        else if (degrees >= 292.5 && degrees < 337.5) return "NW";
        else return "N"; // 337.5 to 360
    }
}

public class TemperatureDetails
{
    [JsonProperty("day")] public double Day { get; set; }
    [JsonProperty("min")] public double Min { get; set; }
    [JsonProperty("max")] public double Max { get; set; }
    [JsonProperty("night")] public double Night { get; set; }
    [JsonProperty("eve")] public double Evening { get; set; }
    [JsonProperty("morn")] public double Morning { get; set; }
}

public class FeelsLikeDetails
{
    [JsonProperty("day")] public double Day { get; set; }
    [JsonProperty("night")] public double Night { get; set; }
    [JsonProperty("eve")] public double Evening { get; set; }
    [JsonProperty("morn")] public double Morning { get; set; }
}

public class WeatherAlert
{
    [JsonProperty("sender_name")] public string SenderName { get; set; }
    [JsonProperty("event")] public string Event { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("start")] public DateTime Start { get; set; }
    
    [JsonConverter(typeof(UnixTimestampConverter))]
    [JsonProperty("end")] public DateTime End { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("tags")] public List<string> Tags { get; set; }
}