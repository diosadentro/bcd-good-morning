using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface ILLMClient
{
    Task<string> GetResponseAsync(LLMRequest request);
}