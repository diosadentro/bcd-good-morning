namespace BcdGoodMorning.Interfaces;

public interface IHolidayService
{
    Task<string> GetTodaysHolidays();
}