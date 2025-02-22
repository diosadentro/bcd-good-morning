using BcdGoodMorning.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BcdGoodMorning.Services;

public class EmailService : IEmailService
{
    private readonly ConfigurationOptions _options;
    private ILogger<EmailService> _logger;
    
    public EmailService(ILogger<EmailService> logger, IOptions<ConfigurationOptions> options)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmail(string name, string toAddress, string subject, string emailBody)
    {
        try
        {
            using (var smtpClient = new SmtpClient())
            {
                if (_options.SmtpUseSsl)
                {
                    await smtpClient.ConnectAsync(_options.SmtpServer, _options.SmtpPort,
                        SecureSocketOptions.SslOnConnect);
                }
                else
                {
                    await smtpClient.ConnectAsync(_options.SmtpServer, _options.SmtpPort);
                }

                await smtpClient.AuthenticateAsync(_options.SmtpUsername, _options.SmtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Good Morning", _options.SmtpFromAddress));
                message.To.Add(new MailboxAddress(name, toAddress));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                var response = await smtpClient.SendAsync(message);

                _logger.LogInformation($"Email sent! Message details: {response}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {toAddress}");
        }
    }
}