using Newtonsoft.Json;

namespace BcdGoodMorning.Models;

public class Meta
{
    public int Code { get; set; }
}

public class DateTimeInfo
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}

public class DateInfo
{
    public string Iso { get; set; }
    public DateTimeInfo Datetime { get; set; }
}

public class Holiday
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateInfo Date { get; set; }
    public List<string> Type { get; set; }
}

public class Response
{
    public List<Holiday> Holidays { get; set; }

    public Response()
    {
        Holidays = new List<Holiday>();
    }
}

public class HolidayModel
{
    public Meta Meta { get; set; }
    
    [JsonConverter(typeof(HolidayJsonConverter))]
    public Response Response { get; set; }
}