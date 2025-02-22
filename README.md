# BcdGoodMorning

BcdGoodMorning is a news digest app that supports Ollama, OpenAI, and Groq to help start your day with the latest news.

## Features

- **Article Parsing**: Uses a separate article parsing container which uses @postlight/parser to pull information. See [here](https://github.com/diosadentro/bcd-article-parser/tree/main) to setup this container.
- **AI Integration**: Supports multiple AI services including Ollama, OpenAI, and Groq.
- **Customizable**: Customize the number of recipients, weather zipcode, etc via the appsettings.json file or Environment Variables.

## Dependent Services
- ArtService: Pulls a daily piece of art using the [Art Institute of Chicago API](https://api.artic.edu/docs/)
- HolidayService: Pulls today's holidays (if any) from [Calendarific API](https://calendarific.com/)
- WeatherService: Pulls today's weather and the weather forecast from [openweathermap](https://openweathermap.org/). It uses [openstreetmap](https://www.openstreetmap.org/) to convert zipcode to lat/long for openweathermap.
- WikipediaService: Uses the [Wikipedia Rest API](https://en.wikipedia.org/api/rest_v1/) to get an article of the day.
- WordOfTheDayService: Uses the [Merriam-Webster's Word of the Day](https://www.merriam-webster.com/wotd/feed/rss2) RSS feed to pull the word of the day.
- SalutationService: Uses a registered LLM agent to generate a salutation to start the email
- NewsService: Uses a registered LLM agent to convert news articles into bite-sized summaries for consumption
- EmailService: Utilizes an SMTP relay to send the daily digest emails to recipients

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker (optional, for containerized deployment)
- A running service hosting [bcd-article-parser](https://github.com/diosadentro/bcd-article-parser/tree/main)
- Registrations and API keys for the services mentioned above.

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/diosadentro/bcd-good-morning.git
    cd bcd-good-morning
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Setup appsettings.json file (available as well through environment variables). Details are below

4. Build the project:
    ```sh
    dotnet build
    ```

### Configuration

Configure the application settings in `appsettings.json` and `appsettings.Development.json` as follows:

#### RssNewsFeedUrls
Articles are pulled via RSS feeds. This service supports both google RSS feeds as well as custom news RSS feeds as long as the follow the basic article google format.

Provide each feed as an item in the list. Example below.
**Url**: URL of the RSS Feed
**Weight** Ordering of articles in the digest email
**MaxItems** Optional flag to limit the number of items by date.

```
"RssNewsFeedUrls": [
      { "Url": "https://news.google.com/rss/search?q=%22technology+llm%22&hl=en-US&gl=US&ceid=US:en", "Weight": 3, "MaxItems": 2 },
    ]
```

#### LLMModelName
The name of the LLM model to use when setting up OpenAI, Groq, or OLlama

```
"LLMModelName": "llama-3.3-70b-specdec"
```

#### LLMEngineType
Supports the following strings:
1. groq
2. openai
3. ollama

#### ArticleParseUrl
The URL of the postlight parser. Do not include the /parse path. Example:
```
"ArticleParseUrl": "http://localhost:3000",
```

#### SMTP Settings
The SMTP settings for from address, port, server url, username, and password.
```
    "SmtpFromAddress": "no-reply@demo.com",
    "SmtpServer": "demo.server.com",
    "SmtpPort": 465,
    "SmtpUseSsl": true,
```

#### TimeZone
The .NET Timezone used to convert weather dates. A full list is available [here](https://stackoverflow.com/questions/14149346/what-value-should-i-pass-into-timezoneinfo-findsystemtimezonebyidstring)

#### ReportEmailAddress
Email address used in the user agent for the Art of Chicago API. They'll use this to communicate with you if there's a problem.

#### Recipients
List of recipients to send the digest email to. The Location is just a string location used in the email digest when displaying the weather forecast. Example:

```
"Recipients": [
      {
        "Email": "me@demo.com",
        "Name": "John Doe",
        "Zipcode": "99999",
        "Location": "Outer Space"
      }
    ]
```

#### WeatherApiKey
API key generated from [openweathermap](https://openweathermap.org/)

```
"WeatherApiKey": "secret",
```

#### HolidayApiKey
API key generated from [Calendarific API](https://calendarific.com/)

```
"HolidayApiKey": "secret",
``` 

#### SmtpUsername
Username passed during SMTP Authentication

```
"SmtpUsername": "user",
``` 

#### SmtpPassword
Password passed during SMTP Authentication

```
"SmtpPassword": "secret",
``` 

#### LLMApiKey
API key used to authenticate to LLM provider. Only used for groq and openai.

```
"LLMApiKey": "secret",
``` 

### Running the Application

To run the application locally:
```sh
dotnet run --project BcdGoodMorning/BcdGoodMorning.csproj
```

### Docker Deployment
You can use the docker compose file to generate a docker image.
Environment variables in the appsettings.Development.json (secret values) can be passed using the [Microsoft Configuration Format](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-9.0#non-prefixed-environment-variables). For example:

```
ConfigurationOptions__HolidayApiKey
ConfigurationOptions__LLMApiKey
ConfigurationOptions__WeatherApiKey
ConfigurationOptions__SmtpUsername
ConfigurationOptions__SmtpPassword
```


# License
This project is licensed under the MIT License. See the LICENSE file for details.