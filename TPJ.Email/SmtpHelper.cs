using System.Net;
using System.Net.Mail;

namespace TPJ.Email;

internal static class SmtpHelper
{
    public static bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith('.'))        
            return false;
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }

    public static SmtpClient CreateClient(IEmailSettings emailSettings)
    {
        ArgumentNullException.ThrowIfNull(emailSettings);

        if (string.IsNullOrWhiteSpace(emailSettings.SmtpClient))
            throw new ArgumentException("SMTP client address is required.", nameof(emailSettings));

        var smtpClient = new SmtpClient(emailSettings.SmtpClient)
        {
            EnableSsl = emailSettings.EnableSSL
        };

        if (emailSettings.Port.HasValue)
            smtpClient.Port = emailSettings.Port.Value;

        if (!string.IsNullOrWhiteSpace(emailSettings.SmtpUser)
               && !string.IsNullOrWhiteSpace(emailSettings.SmtpPassword))
        {
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.SmtpPassword);
        }

        return smtpClient;
    }

    public static (MailMessage mailMessage, List<MemoryStream> streams) CreateMessage(CreateEmailBatch emailDetails, CreateEmailEmail email)
    {
        ArgumentNullException.ThrowIfNull(emailDetails);

        ArgumentNullException.ThrowIfNull(email);

        if (emailDetails.From is null)
            throw new ArgumentException("From address is required.", nameof(emailDetails));

        if (email.To is null || !email.To.Any())
            throw new ArgumentException("At least one recipient is required.", nameof(email));

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailDetails.From.Email, emailDetails.From.DisplayName),
            Subject = email.Subject ?? emailDetails.Subject,
            Body = email.Body ?? emailDetails.Body,
            IsBodyHtml = true,
        };

        foreach (var to in email.To)
            mailMessage.To.Add(new MailAddress(to.Email, to.DisplayName));

        if (email.CC is not null)
        {
            foreach (var cc in email.CC)
                mailMessage.CC.Add(new MailAddress(cc.Email, cc.DisplayName));
        }

        if (email.BCC is not null)
        {
            foreach (var bcc in email.BCC)
                mailMessage.Bcc.Add(new MailAddress(bcc.Email, bcc.DisplayName));
        }

        var streams = AddAttachments(emailDetails, mailMessage);

        return (mailMessage, streams);
    }

    private static List<MemoryStream> AddAttachments(CreateEmailBatch emailDetails, MailMessage mailMessage)
    {
        if (emailDetails.Attachments is null || !emailDetails.Attachments.Any())
            return [];

        var streams = new List<MemoryStream>();
        foreach (var attachment in emailDetails.Attachments)
        {
            if (string.IsNullOrWhiteSpace(attachment.FileName))
                throw new ArgumentException("Attachment filename is required.");

            var stream = new MemoryStream(attachment.FileBytes ?? File.ReadAllBytes(attachment.FilePath!));
            stream.Seek(0, SeekOrigin.Begin);

            mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
            streams.Add(stream);
        }

        return streams;
    }

    public static void CloseStreams(List<MemoryStream> streams)
    {
        if (streams is null)
            return;

        foreach (var stream in streams)
            stream?.Dispose();
    }
}
