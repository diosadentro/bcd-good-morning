using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BcdGoodMorning.Services;

public class HolidayService : IHolidayService
{
    private readonly ILogger<HolidayService> _logger;
    private readonly ConfigurationOptions _config;

    public HolidayService(ILogger<HolidayService> logger, IOptions<ConfigurationOptions> config)
    {
        _logger = logger;
        _config = config.Value;
    }
    
    public async Task<string> GetTodaysHolidays()
    {
        var holidayResponse = "";
        using (var client = new HttpClient())
        {
            try
            {
                var response =
                    await client.GetAsync(
                        $"https://calendarific.com/api/v2/holidays?api_key={_config.HolidayApiKey}&country=US&year={DateTime.Today.Year.ToString()}&day={DateTime.Today.Day.ToString()}&month={DateTime.Today.Month.ToString()}&type=national,religious,observance");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from holiday API: {content}", content);
                var holidays = JsonConvert.DeserializeObject<HolidayModel>(content);
                if (holidays.Response.Holidays.Count > 0)
                {
                    holidayResponse += "Today's Holidays:\n";
                    foreach(var holiday in holidays.Response.Holidays)
                    {
                        holidayResponse += $"{holiday.Name} - {holiday.Description}\n";
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException)
            {
                _logger.LogError("No holidays for today");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting holiday for today");
            }
        }
        return holidayResponse;
    }
}