# TPJ.Email
Simple email library that can send html and attachments 

# Configuration
For most cases you'll be using DI to setup the email configuration but you can simple create a new `Emailer` object and pass in a `EmailSettings` with your config if you require

```
var emailer = new Emailer(new EmailSettings() {
... your settings...
});

emailer.Send(...);
```

For the normal DI setup open your appsettings.json and add a TPJ -> Email section, within this you need to configure some settings

Required:
- `SmtpClient` - SMTP server which e-mails will be sent from e.g. smtp.gmail.com
- `From` - e-mails are sent from this account (can be overridden when sending in code)

Optional:
- `SmtpUser` - Send e-mail authing with the given user name
- `SmtpPassword` - Send e-mail authing with the given password
- `FromDisplayName` - e-mails are sent with this value as the display name (can be overridden when sending in code)
- `EnableSSL` - Use SSL when sending the e-mail
- `Port` - Port to send from e.g. 587 (SSL port)

```
{
  "TPJ": {
    "Email": {
      "SmtpClient": "smtp.gmail.com",
      "SmtpUser": "",
      "SmtpPassword": "",
      "From": "",
      "FromDisplayName": "Test Display Name",
      "EnableSSL": true,
      "Port": 587
    }
  }
}
```

Now within your `Startup` method call `services.AddTPJEmail();` now you can inject `TPJ.IEmailer`. See github for an example for both a console and API using DI.

### Simple Console App
```
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TPJ.Email;

namespace TPJ.EmailTest;

class Program
{
    private static IEmailer _emailer = default!;

    static async Task Main(string[] args)
    {
        SetUp();

        _emailer.Send("test@test.com", "Test TPJ Email", "<h1>This is a test sent from a console app</h1>", true);
    }

    private static void SetUp()
    {
        var builder = new ConfigurationBuilder()
              .SetBasePath(System.IO.Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddTPJEmail();

        var serviceProvider = services.BuildServiceProvider();

        _emailer = serviceProvider.GetRequiredService<IEmailer>();
    }
}
```