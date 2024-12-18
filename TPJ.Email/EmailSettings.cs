using Microsoft.Extensions.Configuration;

namespace TPJ.Email;

public interface IEmailSettings
{
    string SmtpClient { get; set; }
    string? SmtpUser { get; set; }
    string? SmtpPassword { get; set; }
    string From { get; set; }
    string? FromDisplayName { get; set; }
    int? Port { get; set; }
    bool EnableSSL { get; set; }
}

public class EmailSettings : IEmailSettings
{
    public string SmtpClient { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public string From { get; set; }
    public string? FromDisplayName { get; set; }
    public int? Port { get; set; }
    public bool EnableSSL { get; set; }

    public EmailSettings()
    {
    }

    public EmailSettings(IConfiguration configuration)
    {
        var smtpClient = configuration["TPJ:Email:SmtpClient"];

        if (string.IsNullOrWhiteSpace(smtpClient))
            throw new ArgumentException("SMTP client missing");

        SmtpClient = smtpClient;

        var from = configuration["TPJ:Email:From"];

        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From address missing");

        From = from;
        FromDisplayName = configuration["TPJ:Email:FromDisplayName"];

        SmtpUser = configuration["TPJ:Email:SmtpUser"];
        SmtpPassword = configuration["TPJ:Email:SmtpPassword"];

        if (bool.TryParse(configuration["TPJ:Email:EnableSSL"], out var enableSSL))
            EnableSSL = enableSSL;

        if (int.TryParse(configuration["TPJ:Email:Port"], out var portNumber))
            Port = portNumber;
    }
}
