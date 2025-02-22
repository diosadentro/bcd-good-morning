using System.ComponentModel.DataAnnotations;
using ThirdParty.Json.LitJson;

namespace BcdGoodMorning.Models;

public class NewSource
{
    [Required]
    public string Url { get; set; }
    
    [Required]
    public int Weight { get; set; }
    
    public int MaxItems { get; set; }
    
    [Required]
    public string Id { get; set; }
}