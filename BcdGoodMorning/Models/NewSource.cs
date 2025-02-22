using System.ComponentModel.DataAnnotations;

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