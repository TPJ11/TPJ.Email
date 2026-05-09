namespace TPJ.Email;

public class CreateEmailSingle
{
    /// <summary>
    /// Who the email is from, if null the value in TPJ:Email:From will be used.
    /// </summary>
    public CreateEmailAudience? From { get; set; }

    /// <summary>
    /// The subject of the email.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// The body of the email (HTML).
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Who the email is to, must have at least one email address.
    /// All email addresses in this list are sent the same email 
    /// and can see each others email addresses.
    /// </summary>
    public required IEnumerable<CreateEmailAudience> To { get; set; }

    /// <summary>
    /// Who the email is CC'd to, can be null.
    /// All email addresses in this list are sent the same email 
    /// and can see each others email addresses.
    /// </summary>
    public IEnumerable<CreateEmailAudience>? CC { get; set; }

    /// <summary>
    /// Who the email is BCC'd to, can be null.
    /// All email addresses in this list are sent the same email 
    /// and their email address is not shown to anyone else within the email
    /// </summary>
    public IEnumerable<CreateEmailAudience>? BCC { get; set; }

    /// <summary>
    /// Any attachments to be sent with the email, can be null.
    /// </summary>
    public IEnumerable<CreateEmailAttachment>? Attachments { get; set; }

    /// <summary>
    /// Converts this single email to a batch email.
    /// </summary>
    /// <returns>Batch email object</returns>
    public CreateEmailBatch ToBatch() =>
        new()
        {
            From = From,
            Subject = Subject,
            Body = Body,
            Attachments = Attachments,
            Emails =
            [
                new()
                {
                    To = To,
                    CC = CC,
                    BCC = BCC,
                }
            ]
        };
}

public class CreateEmailBatch
{
    /// <summary>
    /// Who the email is from, if null the value in TPJ:Email:From will be used
    /// </summary>
    public CreateEmailAudience? From { get; set; }

    /// <summary>
    /// The subject of the email.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// The body of the email (HTML).
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// The emails to be sent, must have at least one email.
    /// </summary>
    public required IEnumerable<CreateEmailEmail> Emails { get; set; }

    /// <summary>
    /// Any attachments to be sent with the email, can be null.
    /// </summary>
    public IEnumerable<CreateEmailAttachment>? Attachments { get; set; }
}

public class CreateEmailEmail
{
    /// <summary>
    /// Overrides the subject of the email in the outer object, 
    /// used for sending personalised emails in a batch.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Overrides the body of the email in the outer object, 
    /// used for sending personalised emails in a batch.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Who the email is to, must have at least one email address.
    /// All email addresses in this list are sent the same email 
    /// and can see each others email addresses.
    /// </summary>
    public required IEnumerable<CreateEmailAudience> To { get; set; }

    /// <summary>
    /// Who the email is CC'd to, can be null.
    /// All email addresses in this list are sent the same email 
    /// and can see each others email addresses.
    /// </summary>
    public IEnumerable<CreateEmailAudience>? CC { get; set; }

    /// <summary>
    /// Who the email is BCC'd to, can be null.
    /// All email addresses in this list are sent the same email 
    /// and their email address is not shown to anyone else within the email
    /// </summary>
    public IEnumerable<CreateEmailAudience>? BCC { get; set; }
}

public class CreateEmailAudience
{
    /// <summary>
    /// The email address of the recipient.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The display name of the recipient, can be null.
    /// </summary>
    public string? DisplayName { get; set; }
}

public class CreateEmailAttachment
{
    /// <summary>
    /// The name of the file to be attached.
    /// Must be set if <see cref="FileBytes"/> is set.
    /// If <see cref="FilePath"/> is set and this value 
    /// is null the file name set by extracting it from the file path.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// The content type of the file to be attached.
    /// If null the content type will be determined 
    /// from the file extension within <see cref="FileName"/>.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The bytes of the file to be attached.
    /// </summary>
    public byte[]? FileBytes { get; set; }

    /// <summary>
    /// The path to the file to be attached.
    /// </summary>
    public string? FilePath { get; set; }
}