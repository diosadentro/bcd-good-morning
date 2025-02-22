using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface IArticleParseClient
{
    Task<NewsArticleParse> GetArticleSummaryAsync(string url);
}