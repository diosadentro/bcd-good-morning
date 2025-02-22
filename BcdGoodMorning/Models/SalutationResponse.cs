using Newtonsoft.Json;

namespace BcdGoodMorning.Models;

public class SalutationResponse
{
    [JsonProperty("salutation")]
    public string Salutation { get; set; }
}