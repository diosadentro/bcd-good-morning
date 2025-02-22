using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface INewsService
{
    Task<List<NewsArticle>> GetNewsAsync();
}