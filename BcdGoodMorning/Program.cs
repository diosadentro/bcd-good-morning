using BcdGoodMorning;
using BcdGoodMorning.Clients;
using BcdGoodMorning.Interfaces;
using BcdGoodMorning.Models;
using BcdGoodMorning.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<ConfigurationOptions>(builder.Configuration.GetSection("ConfigurationOptions"));
builder.Services.AddHostedService<Worker>();

// Services
builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddSingleton<INewsService, NewsService>();
builder.Services.AddSingleton<IWikipediaService, WikipediaService>();
builder.Services.AddSingleton<IWordOfTheDayService, WordOfTheDayService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ISalutationService, SalutationService>();
builder.Services.AddSingleton<IArtService, ArtService>();
builder.Services.AddSingleton<IArticleParseClient, ArticleParseClient>();
builder.Services.AddSingleton<IHolidayService, HolidayService>();

// Get config
var configOptions = builder.Configuration.GetSection("ConfigurationOptions").Get<ConfigurationOptions>();

// Conditionally configure LLM Client based on type
switch (configOptions.LLMEngineType)
{
    case LLMType.Groq:
        builder.Services.AddSingleton<ILLMClient, GroqLLMClient>();
        break;
    case LLMType.OpenAI:
        builder.Services.AddSingleton<ILLMClient, OpenAiClient>();
        break;
    case LLMType.Ollama:
        builder.Services.AddSingleton<ILLMClient, OllamaClient>();
        break;
}

// Really not the best but we want to get the timezone details from env so we can use them when constructing the weather data response
// This can be improved by setting UTC in the weather object and then converting it in the template.
Environment.SetEnvironmentVariable("TZ", configOptions.TimeZone);

var host = builder.Build();
host.Run();