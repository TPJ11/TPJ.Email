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
    bool Debug { get; set; }
}

public class EmailSettings : IEmailSettings
{
    public required string SmtpClient { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public required string From { get; set; }
    public string? FromDisplayName { get; set; }
    public int? Port { get; set; }
    public bool EnableSSL { get; set; }
    public bool Debug { get; set; }

    public EmailSettings()
    {
    }

    public EmailSettings(IConfiguration configuration)
    {
        if (bool.TryParse(configuration["TPJ:Email:Debug"], out var debug))
            Debug = debug;

        if (configuration["TPJ:Email:SmtpClient"] is not null)        
            SmtpClient = configuration["TPJ:Email:SmtpClient"]!;        
        else
        {
            var azureKeyVaultName = configuration["TPJ:Email:AzureKeyVault:SmtpClient"]!;

            if (!string.IsNullOrWhiteSpace(azureKeyVaultName))
                SmtpClient = TPJ.Encrypt.AzureKeyVault.GetSecretValue(configuration, azureKeyVaultName);
        }

        var from = configuration["TPJ:Email:From"];

        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From address missing");

        From = from;
        FromDisplayName = configuration["TPJ:Email:FromDisplayName"];

        SmtpUser = configuration["TPJ:Email:SmtpUser"];
        if (string.IsNullOrWhiteSpace(SmtpUser))
        {
            var azureKeyVaultName = configuration["TPJ:Email:AzureKeyVault:SmtpUser"]!;

            if (!string.IsNullOrWhiteSpace(azureKeyVaultName))
                SmtpUser = TPJ.Encrypt.AzureKeyVault.GetSecretValue(configuration, azureKeyVaultName);
        }

        SmtpPassword = configuration["TPJ:Email:SmtpPassword"];
        if (string.IsNullOrWhiteSpace(SmtpPassword))
        {
            var azureKeyVaultName = configuration["TPJ:Email:AzureKeyVault:SmtpPassword"]!;

            if (!string.IsNullOrWhiteSpace(azureKeyVaultName))
                SmtpPassword = TPJ.Encrypt.AzureKeyVault.GetSecretValue(configuration, azureKeyVaultName);
        }

        if (bool.TryParse(configuration["TPJ:Email:EnableSSL"], out var enableSSL))
            EnableSSL = enableSSL;

        if (int.TryParse(configuration["TPJ:Email:Port"], out var portNumber))
            Port = portNumber;
    }
}
