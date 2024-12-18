using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace TPJ.Email;

public interface IEmailer
{
    /// <summary>
    /// Send the given message to the given person
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="fromDisplayName">Name shown on the email instead of the email address</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(string to, string subject, string message, bool isHtml,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);

    /// <summary>
    /// Send the given message to the given list of people
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="fromDisplayName">Name shown on the email instead of the email address</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(IEnumerable<string> to, string subject, string message, bool isHtml,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);

    /// <summary>
    /// Send the given message to the given list of people
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="attachment">Attachment to send with the e-mail</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(IEnumerable<string> to, string subject, string message, bool isHtml, FileAttachment attachment,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);

    /// <summary>
    /// Send the given message to the given list of people
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="attachment">Attachment to send with the e-mail</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="fromDisplayName">Name shown on the email instead of the email address</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(string to, string subject, string message, bool isHtml, FileAttachment attachment,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);

    /// <summary>
    /// Send the given message to the given list of people
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="attachments">Attachments to send with the e-mail</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="fromDisplayName">Name shown on the email instead of the email address</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(IEnumerable<string> to, string subject, string message, bool isHtml, IEnumerable<FileAttachment> attachments,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);

    /// <summary>
    /// Send the given message to the given list of people
    /// </summary>
    /// <param name="to">Who to send the e-mail to (e-mail address)</param>
    /// <param name="from">Who is sending the e-mail (e-mail address)</param>
    /// <param name="subject">Subject of the e-mail</param>
    /// <param name="message">Content of the e-mail</param>
    /// <param name="isHtml">If the content is HTML</param>
    /// <param name="attachments">Attachments to send with the e-mail</param>
    /// <param name="from">Who the e-mail is from</param>
    /// <param name="fromDisplayName">Name shown on the email instead of the email address</param>
    /// <param name="cc">List of e-mails to CC in on the e-mail</param>
    /// <param name="bcc">List of e-mails to BCC in on the e-mail</param>
    void Send(string to, string subject, string message, bool isHtml, IEnumerable<FileAttachment> attachments,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null);
}

public class Emailer : IEmailer
{
    private readonly IEmailSettings _emailSettings;
    private readonly SmtpClient _smtpClient;
    private List<MemoryStream> _streamsToClose = new();

    public Emailer(IEmailSettings emailSettings)
    {
        _emailSettings = emailSettings;

        _smtpClient = new SmtpClient(_emailSettings.SmtpClient)
        {
            EnableSsl = _emailSettings.EnableSSL,
            Port = _emailSettings.Port ?? 25,
        };

        if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUser)
            && !string.IsNullOrWhiteSpace(_emailSettings.SmtpPassword))
        {
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new System.Net.NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }
    }

    public void Send(string to, string subject, string message, bool isHtml,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) =>
        SendEmail(message, subject, new List<string>() { to }, isHtml, from, fromDisplayName, null, cc, bcc);

    public void Send(IEnumerable<string> to, string subject, string message, bool isHtml,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) => 
        SendEmail(message, subject, to, isHtml, from, fromDisplayName, null, cc, bcc);

    public void Send(IEnumerable<string> to, string subject, string message, bool isHtml, FileAttachment attachment, 
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) =>
        SendEmail(message, subject, to, isHtml, from, fromDisplayName, new List<FileAttachment> { attachment }, cc, bcc);

    public void Send(string to, string subject, string message, bool isHtml, FileAttachment attachment, 
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) => 
        SendEmail(message, subject, new List<string>() { to }, isHtml, from, fromDisplayName, new List<FileAttachment> { attachment }, cc, bcc);   

    public void Send(IEnumerable<string> to, string subject, string message, bool isHtml, IEnumerable<FileAttachment> attachments,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) => 
        SendEmail(message, subject, to, isHtml, from, fromDisplayName, attachments, cc, bcc);

    public void Send(string to, string subject, string message, bool isHtml, IEnumerable<FileAttachment> attachments,
        string? from = null, string? fromDisplayName = null, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null) => 
        SendEmail(message, subject, new List<string>() { to }, isHtml, from, fromDisplayName, attachments, cc, bcc);

    /// <summary>
    /// Send an e-mail using the given message body and subject. 
    /// Sending the e-mails to each person in the e-mail list
    /// </summary>
    /// <param name="messageBody">E-mail Message body</param>
    /// <param name="subject">E-mail Subject</param>
    /// <param name="emailToList">E-mail To</param>
    /// <param name="from">E-mail From</param>
    /// <param name="isHtml">is the e-mail HTML</param>
    private void SendEmail(string messageBody, string subject, IEnumerable<string> emailToList,
        bool isHtml, string? from = null, string? fromDisplayName = null, IEnumerable<FileAttachment>? attachments = null,
        IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null)
    {
        if (emailToList is null)
            return;

        try
        {
            from = string.IsNullOrWhiteSpace(from) ? _emailSettings.From : from;
            fromDisplayName = string.IsNullOrWhiteSpace(fromDisplayName) ? _emailSettings.FromDisplayName : fromDisplayName;

            using var message = new MailMessage()
            {
                Subject = subject,
                From = new MailAddress(from, fromDisplayName),
                Body = messageBody,
                IsBodyHtml = isHtml
            };

            // Add each e-mail to send to
            foreach (string email in emailToList)
                message.To.Add(email);

            if (cc is not null)
            {
                foreach (var email in cc)
                    message.CC.Add(email);
            }

            if (bcc is not null)
            {
                foreach (var email in bcc)
                    message.Bcc.Add(email);
            }

            if (attachments is not null)
            {
                foreach (var attachment in attachments)
                    message.Attachments.Add(CreateAttachment(attachment));
            }

            // Attempt to send the email 3 times then throw the error
            var hasSent = false;
            var sendCount = 0;
            while (!hasSent)
            {
                try
                {
                    _smtpClient.Send(message);
                    hasSent = true;
                }
                catch (Exception)
                {
                    sendCount++;
                    if (sendCount >= 3)
                        throw;
                    else
                        Thread.Sleep(100);
                }
            }
        }
        finally
        {
            CloseStreams();
        }
    }
    
    /// <summary>
    /// Creates an attachment for the e-mail
    /// </summary>
    /// <param name="details">Attachment details</param>
    /// <returns>Attachment</returns>
    private Attachment CreateAttachment(FileAttachment details)
    {
        Attachment attachment;

        if (string.IsNullOrWhiteSpace(details.FilePath))
        {
            var stream = new MemoryStream(details.FileBytes);

            stream.Seek(0, SeekOrigin.Begin);

            attachment = new Attachment(stream, details.FileName, details.MimeType);
            // Add time stamp information for the file.
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = DateTime.Now;
            disposition.ModificationDate = DateTime.Now;
            disposition.ReadDate = DateTime.Now;

            _streamsToClose.Add(stream);
        }
        else
        {
            attachment = new Attachment(details.FilePath, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(details.FilePath);
            disposition.ModificationDate = File.GetLastWriteTime(details.FilePath);
            disposition.ReadDate = File.GetLastAccessTime(details.FilePath);
        }
        
        return attachment;
    }

    /// <summary>
    /// Closes all streams and clears the stream list
    /// </summary>
    private void CloseStreams()
    {
        foreach (var stream in _streamsToClose)
        {
            stream.Close();
            stream.Dispose();
        }

        _streamsToClose.Clear();
    }
}
