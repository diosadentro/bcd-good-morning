using BcdGoodMorning.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BcdGoodMorning;

public class HolidayJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Response);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            return new Response { Holidays = token.ToObject<List<Holiday>>() };
        }
        else if (token.Type == JTokenType.Object)
        {
            return new Response();
        }
        throw new JsonSerializationException("Unexpected token type: " + token.Type);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var response = (Response)value;
        serializer.Serialize(writer, response.Holidays);
    }
}