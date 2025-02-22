using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface IWikipediaService
{
    Task<WikipediaData> GetRandomWikipediaArticleAsync();
}