# TPJ.Email

Simple SMTP email library for .NET that supports HTML emails, attachments, single sends, and batch sends.

## Install

```bash
dotnet add package TPJ.Email
```

## Configuration

Add your SMTP settings to `appsettings.json`.

```json
{
  "TPJ": {
    "Email": {
      "SmtpClient": "smtp.example.com",
      "SmtpUser": "smtp-user",
      "SmtpPassword": "smtp-password",
      "From": "no-reply@example.com",
      "FromDisplayName": "My App",
      "Port": 587,
      "EnableSSL": true,
      "Debug": false
    }
  }
}
```

Supported settings:

- `SmtpClient`
- `SmtpUser`
- `SmtpPassword`
- `From`
- `FromDisplayName`
- `Port`
- `EnableSSL`
- `Debug`

`SmtpClient`, `SmtpUser`, and `SmtpPassword` can also be loaded from Azure Key Vault by using:

- `TPJ:Email:AzureKeyVault:SmtpClient`
- `TPJ:Email:AzureKeyVault:SmtpUser`
- `TPJ:Email:AzureKeyVault:SmtpPassword`

## Register the package

Register `IEmailSettings` from configuration, then add the email services.

```csharp
using TPJ.Email;

builder.Services.AddTPJEmail();
```

## Console app example

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TPJ.Email;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var services = new ServiceCollection();
services.AddTPJEmail();

using var serviceProvider = services.BuildServiceProvider();

var emailer = serviceProvider.GetRequiredService<IEmailer>();

await emailer.SendAsync(new CreateEmailSingle
{
    To =
    [
        new CreateEmailAudience
        {
            Email = "jane@example.com",
            DisplayName = "Jane"
        }
    ],
    Subject = "Hello from a console app",
    Body = "<h1>Email sent from TPJ.Email</h1><p>This email was sent from a console application.</p>"
});
```

## API example

Example with a minimal API:

```csharp
using TPJ.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEmailSettings>(_ => new EmailSettings(builder.Configuration));
builder.Services.AddTPJEmail();

var app = builder.Build();

app.MapPost("/emails/test", async (string to, IEmailer emailer, CancellationToken cancellationToken) =>
{
    await emailer.SendAsync(new CreateEmailSingle
    {
        To =
        [
            new CreateEmailAudience
            {
                Email = to
            }
        ],
        Subject = "Hello from the API",
        Body = "<p>This email was sent from an ASP.NET Core API.</p>"
    }, cancellationToken);

    return Results.Accepted();
});

app.Run();
```

## Batch email example

Use `SendBatchAsync` when you want to send the same base email to multiple recipients, with optional per-recipient subject or body overrides.

```csharp
await emailer.SendBatchAsync(new CreateEmailBatch
{
    From = new CreateEmailAudience
    {
        Email = "no-reply@example.com",
        DisplayName = "My App"
    },
    Subject = "Weekly update",
    Body = "<p>Default email body</p>",
    Emails =
    [
        new CreateEmailEmail
        {
            To =
            [
                new CreateEmailAudience { Email = "alice@example.com", DisplayName = "Alice" }
            ]
        },
        new CreateEmailEmail
        {
            To =
            [
                new CreateEmailAudience { Email = "bob@example.com", DisplayName = "Bob" }
            ],
            Subject = "Weekly update for Bob",
            Body = "<p>Custom body for Bob</p>"
        }
    ]
});
```

## Attachments

Attachments can be added by file path or byte array.

```csharp
await emailer.SendAsync(new CreateEmailSingle
{
    To =
    [
        new CreateEmailAudience { Email = "jane@example.com" }
    ],
    Subject = "Report",
    Body = "<p>Please find the report attached.</p>",
    Attachments =
    [
        new CreateEmailAttachment
        {
            FilePath = "Reports/report.pdf"
        }
    ]
});
```

## Notes

- Email bodies are sent as HTML.
- If `From` is omitted on the email request, the configured `TPJ:Email:From` value is used.
- Each `CreateEmailEmail` in a batch is sent separately.
- `To` must contain at least one valid email address.
