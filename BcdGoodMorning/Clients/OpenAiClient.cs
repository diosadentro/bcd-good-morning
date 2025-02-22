using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace BcdGoodMorning.Clients;

public class OpenAiClient : ILLMClient
{
    private readonly string _modelName;
    private readonly string _apiKey;
    public OpenAiClient(IOptions<ConfigurationOptions> options)
    {
        _modelName = options.Value.LLMModelName;
        _apiKey = options.Value.LLMApiKey;
    }

    public async Task<string> GetResponseAsync(LLMRequest request)
    {
        ChatClient client = new(model: _modelName, apiKey: _apiKey);
        var prompt = $"{request.SystemPrompt} {request.UserPrompt}";
        var completion = await client.CompleteChatAsync(new ChatMessage[]
        {
            prompt
        }, new ChatCompletionOptions()
        {
            Temperature = 0f,
            FrequencyPenalty = 0.25f,
            Tools = {  }
        });

        return completion.Value.Content[0].Text;
    }
}