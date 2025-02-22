namespace BcdGoodMorning.Interfaces;

public interface IEmailService
{
    Task SendEmail(string name, string toAddress, string subject, string emailBody);
}