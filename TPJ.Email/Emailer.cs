namespace TPJ.Email;

public interface IEmailer
{
    /// <summary>
    /// Sends a single email.
    /// </summary>
    /// <param name="emailDetails">Email Details</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    Task SendAsync(CreateEmailSingle emailDetails, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a batch of emails.
    /// </summary>
    /// <param name="emailDetails">Email Details</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    Task SendBatchAsync(CreateEmailBatch emailDetails, CancellationToken cancellationToken = default);
}

// <inheritdoc />
public class Emailer(IEmailSettings _emailSettings) : IEmailer
{
    public async Task SendAsync(CreateEmailSingle emailDetails, CancellationToken cancellationToken = default) =>
        await SendBatchAsync(emailDetails.ToBatch(), cancellationToken).ConfigureAwait(false);

    public async Task SendBatchAsync(CreateEmailBatch emailDetails, CancellationToken cancellationToken = default)
    {
        UpdateAttachmentDetails(emailDetails);
        UpdateFromAddress(emailDetails);
        ValidateEmailDetails(emailDetails);

        await SendSmtpAsync(emailDetails).ConfigureAwait(false);

        Debug.WriteLine(_emailSettings.Debug, string.Empty);
    }

    private void UpdateFromAddress(CreateEmailBatch emailDetails)
    {
        if (emailDetails.From is not null || string.IsNullOrWhiteSpace(_emailSettings.From))
            return;

        emailDetails.From = new CreateEmailAudience
        {
            Email = _emailSettings.From!
        };
    }

    private void ValidateEmailDetails(CreateEmailBatch emailDetails)
    {
        if (emailDetails.Attachments is not null)
        {
            foreach (var attachment in emailDetails.Attachments)
            {
                if (attachment.FilePath is null && attachment.FileBytes is null)
                    throw new InvalidOperationException("Attachment must have either FileBytes or FileLocation");

                if (attachment.FilePath is not null && !File.Exists(attachment.FilePath))
                    throw new FileNotFoundException("Attachment file not found", attachment.FilePath);

                if (attachment.ContentType is null)
                    throw new InvalidOperationException("Attachment must have ContentType");

                if (attachment.FileName is null)
                    throw new InvalidOperationException("Attachment must have FileName");
            }
        }

        if ((emailDetails.From is null || !SmtpHelper.IsValidEmail(emailDetails.From.Email))
            && _emailSettings.SmtpClient is not null)
            throw new InvalidOperationException("From email required");

        if (emailDetails.Emails is null || emailDetails.Emails.Count() == 0)
            throw new InvalidOperationException("One or more email is required");

        var invalidEmailAddresses = GetInvalidEmailAddresses(emailDetails);
        if (invalidEmailAddresses.Count > 0)
            throw new ArgumentException($"Invalid email addresses: {string.Join(", ", invalidEmailAddresses)}");
    }

    private static List<string> GetInvalidEmailAddresses(CreateEmailBatch emailDetails)
    {
        return emailDetails.Emails
            .SelectMany(email => email.To
                .Concat(email.CC ?? [])
                .Concat(email.BCC ?? []))
            .Where(audience => !SmtpHelper.IsValidEmail(audience.Email))
            .Select(audience => audience.Email)
            .ToList();
    }

    private static void UpdateAttachmentDetails(CreateEmailBatch emailDetails)
    {
        if (emailDetails.Attachments is null)
            return;

        foreach (var attachment in emailDetails.Attachments)
        {
            SetAttachmentContentType(attachment);
            SetAttachmentFileName(attachment);
        }
    }

    private static void SetAttachmentFileName(CreateEmailAttachment attachment)
    {
        if (attachment.FileName is not null)
            return;

        if (attachment.FilePath is not null)
            attachment.FileName = Path.GetFileName(attachment.FilePath);
    }

    private static void SetAttachmentContentType(CreateEmailAttachment attachment)
    {
        if (attachment.ContentType is not null)
            return;

        if (attachment.FileName is not null)
            attachment.ContentType = attachment.FileName.ToContentType();

        if (attachment.ContentType is null && attachment.FilePath is not null)
            attachment.ContentType = attachment.FilePath.ToContentType();
    }

    private async Task SendSmtpAsync(CreateEmailBatch emailDetails)
    {
        Debug.WriteLine(_emailSettings.Debug, "Sending via SMTP");
        using var smtpClient = SmtpHelper.CreateClient(_emailSettings);

        foreach (var email in emailDetails.Emails)
        {
            Debug.WriteLine(_emailSettings.Debug, $"Email sending to: {string.Join(",", email.To.Select(x => x.Email))}");
            var (mailMessage, streams) = SmtpHelper.CreateMessage(emailDetails, email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                Debug.WriteLine(_emailSettings.Debug, "SMTP sent successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine(_emailSettings.Debug, $"SMTP exception - {e.Message}");
                throw;
            }
            finally
            {
                SmtpHelper.CloseStreams(streams);
            }
        }
    }
}
