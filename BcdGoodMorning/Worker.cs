using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using Microsoft.Extensions.Options;
using RazorEngineCore;

namespace BcdGoodMorning;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IWeatherService _weatherService;
    private readonly INewsService _newsService;
    private readonly IWikipediaService _wikipediaService;
    private readonly IWordOfTheDayService _wordOfTheDayService;
    private readonly ISalutationService _salutationService;
    private readonly IEmailService _emailService;
    private readonly IArtService _artService;
    private readonly ConfigurationOptions _config;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private IHolidayService _holidayService;

    public Worker(
        ILogger<Worker> logger, 
        IWeatherService weatherService, 
        INewsService newsService, 
        IWikipediaService wikipediaService, 
        IWordOfTheDayService wordOfTheDayService,
        ISalutationService salutationService,
        IEmailService emailService,
        IArtService artService,
        IOptions<ConfigurationOptions> options,
        IHolidayService holidayService,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _weatherService = weatherService;
        _newsService = newsService;
        _wikipediaService = wikipediaService;
        _wordOfTheDayService = wordOfTheDayService;
        _salutationService = salutationService;
        _emailService = emailService;
        _artService = artService;
        _config = options.Value;
        _applicationLifetime = applicationLifetime;
        _holidayService = holidayService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Generating Digest Email at: {time}", DateTimeOffset.Now);
        }
         
         // Get Wiki learning Article
         var wikipediaArticle = await _wikipediaService.GetRandomWikipediaArticleAsync();
 
         // Get Word of the day
         var wordOfTheDay = await _wordOfTheDayService.GetWordOfTheDayAsync();
         
         // Get Art of the day
         var art = await _artService.GetArtOfTheDay();
         
         // Get Holiday
         var holiday = await _holidayService.GetTodaysHolidays();
         
         // Get News
         var newsArticles = await _newsService.GetNewsAsync();

         if (_config.Recipients.Any())
         {
             foreach (var recipient in _config.Recipients)
             {
                 var weatherData = await _weatherService.GetWeatherAsync(recipient.ZipCode);
                 var today = weatherData.Daily.FirstOrDefault(x => x.DateTime.Date == DateTime.Today);
                 var model = new DailyEmailModel()
                 {
                     Location = recipient.Location,
                     Salutation = await _salutationService.GenerateSalutation(recipient.Name, holiday),
                     NewsArticles = newsArticles,
                     TodayWeather = today,
                     Forecast = weatherData.Daily,
                     WeatherAlerts = weatherData.Alerts,
                     WikipediaData = wikipediaArticle,
                     WordOfTheDay = wordOfTheDay,
                     ArtImage = art
                 };

                 // Get template from file
                 string templatePath =
                     Path.Combine(Directory.GetCurrentDirectory(), "Templates", "DailyEmail.cshtml");
                 if (!File.Exists(templatePath))
                 {
                     _logger.LogError($"Failed to file template file {templatePath}");
                     throw new Exception($"Template file {templatePath} not found");
                 }

                 string templateContents = File.ReadAllText(templatePath);

                 try
                 {
                     // Render the Razor view to HTML string
                     IRazorEngine razorEngine = new RazorEngine();
                     IRazorEngineCompiledTemplate template = razorEngine.Compile(templateContents);

                     string result = template.Run(model);
                     var firstName = recipient.Name.Substring(0, recipient.Name.IndexOf(" "));

                     // Now you can send this HTML in an email
                     var subject = $"Hi {firstName}! Here's your Daily Email for " +
                                   DateTime.Now.ToString("MMMM dd, yyyy");

                     await _emailService.SendEmail(recipient.Name, recipient.Email, subject, result);
                 }
                 catch (Exception ex)
                 {
                     _logger.LogError(ex, "Failed to render Razor template");
                     throw;
                 }
             }
         }
         
         _applicationLifetime.StopApplication();
    }
}