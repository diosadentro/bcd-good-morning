using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.Extensions.Options;

namespace BcdGoodMorning.Clients;

public class GroqLLMClient : ILLMClient
{
    private readonly IGroqClient _client;

    public GroqLLMClient(ILogger<GroqClient> logger, IOptions<ConfigurationOptions> options)
    {
        _client = new GroqClient(options.Value.LLMApiKey, options.Value.LLMModelName)
            .SetTemperature(0);
    }

    public async Task<string> GetResponseAsync(LLMRequest request)
    {
        var response = await _client.CreateChatCompletionAsync(
            new Message { Role = MessageRoleType.System, Content = request.SystemPrompt },
            new Message { Role = MessageRoleType.User, Content = request.UserPrompt }
            );

        return response;
    }

}