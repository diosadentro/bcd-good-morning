using System.ComponentModel.DataAnnotations;
using BcdGoodMorning.Models;

namespace BcdGoodMorning;

public class ConfigurationOptions
{
    [Required]
    public required string WeatherApiKey { get; set; }
    
    [Required]
    public required string TimeZone { get; set; }
    
    [Required]
    public required string ReportEmailAddress { get; set; }
    
    [Required]
    public required List<NewSource> RssNewsFeedUrls { get; set; }
    
    [Required]
    public required string LLMModelName { get; set; }
    
    [Required]
    public required string HolidayApiKey { get; set; }
    
    [Required]
    public required List<Recipient> Recipients { get; set; }
    
    [Required]
    public required string ArticleParseUrl { get; set; }
    
    [Required]
    public required LLMType LLMEngineType { get; set; }
    
    [Required]
    public required string SmtpServer { get; set; }
    
    [Required]
    public required int SmtpPort { get; set; }
    
    [Required]
    public required string SmtpUsername { get; set; }
    
    [Required]
    public required string SmtpPassword { get; set; }
    
    [Required]
    public required bool SmtpUseSsl { get; set; }
    
    [Required]
    public required string SmtpFromAddress { get; set; }
    
    public string LLMUrl { get; set; }
    public string LLMApiKey { get; set; }
    
}