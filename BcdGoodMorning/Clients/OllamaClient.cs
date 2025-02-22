using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace BcdGoodMorning.Clients;

public class OllamaClient : ILLMClient
{
    private readonly Uri _baseUrl;
    private readonly string _modelName;
    private readonly IOptions<ConfigurationOptions> _options;
    private readonly ILogger<OllamaClient> _logger;
    private readonly OllamaApiClient _ollamaApiClient;
    
    public OllamaClient(IOptions<ConfigurationOptions> options, ILogger<OllamaClient> logger)
    {
        _baseUrl = new Uri(options.Value.LLMUrl);
        _modelName = options.Value.LLMModelName;
        _ollamaApiClient = new OllamaApiClient(_baseUrl);
        _ollamaApiClient.SelectedModel = _modelName;
        _logger = logger;
    }
    
    public async Task<string> GetResponseAsync(LLMRequest request)
    {
        var response = "";
        var prompt = $"{request.SystemPrompt} {request.UserPrompt}";
        _logger.LogDebug("Sending prompt to Ollama: {prompt}", prompt);
        await foreach (var stream in _ollamaApiClient.GenerateAsync(prompt))
        {
            response += stream.Response;
        }
        _logger.LogDebug("Received response from Ollama: {response}", response);
        return response;
    }
}