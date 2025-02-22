using Microsoft.Playwright;

namespace BcdGoodMorning.Models;

public class Recipient
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string ZipCode { get; set; }
    public string Location { get; set; }
}