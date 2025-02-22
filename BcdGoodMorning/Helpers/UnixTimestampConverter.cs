using Newtonsoft.Json;

namespace BcdGoodMorning;

public class UnixTimestampConverter : JsonConverter<DateTime>
{
    private readonly string _timeZone;
    public UnixTimestampConverter()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TZ")))
        {
            _timeZone = Environment.GetEnvironmentVariable("TZ");
        }
        else
        {
            _timeZone = "UTC";
        }
    }
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is long unixTimestamp)
        {
            var utcTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;

            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZone);
            
            // Convert the UTC DateTime to the local timezone
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localTimeZone);
        
            return localDateTime;
        }
        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(new DateTimeOffset(value, TimeSpan.Zero).ToUnixTimeSeconds());
    }
}