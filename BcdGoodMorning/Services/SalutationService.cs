using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;

namespace BcdGoodMorning.Services;

public class SalutationService : ISalutationService
{
    private readonly ILogger<SalutationService> _logger;
    private readonly ILLMClient _llmClient;
    private readonly ConfigurationOptions _config;
    
    public SalutationService(ILogger<SalutationService> logger, ILLMClient llmClient, IOptions<ConfigurationOptions> options)
    {
        _logger = logger;
        _llmClient = llmClient;
        _config = options.Value;
    }
    
    public async Task<string> GenerateSalutation(string name, string holiday)
    {
        var date = DateTime.Today.ToString("D");
        var holidayPrompt = !string.IsNullOrWhiteSpace(holiday) ? "Also include details about today's holiday in a separate paragraph: " + holiday : "";
        var firstName = name.Substring(0, name.IndexOf(' '));
        var systemPrompt =
            $"You are a daily email digest system generating an email that contains important information to start the day. " +
            $"Generate a 1 paragraph fun introduction to the email. " +
            $"Start by saying hello to {firstName} and provide the day of the week and the date for: {date}. Bold this information using HTML tags. " +
            holidayPrompt +
            $"Use HTML tags to make the greeting look nice, especially paragraph tags to separate paragraphs. " +
            $"do not return any header. Make sure to be personable, fun and relevant to the day. " +
            $"The greeting should be lighthearted, engaging, and happy.";
        
        var prompt = "Please generate a salutation for the email digest for today with HTML tags. " +
                            "do not return JSON, only return the html.";

        var request = new LLMRequest()
        {
            SystemPrompt = systemPrompt,
            UserPrompt = prompt
        };
        var rawResponse = await _llmClient.GetResponseAsync(request);
        return rawResponse;
    }
}