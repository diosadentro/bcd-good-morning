using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface IWordOfTheDayService
{
    Task<WordOfTheDay> GetWordOfTheDayAsync();
}