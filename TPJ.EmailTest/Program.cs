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

        _emailer.Send(GetToEmail(), "Test TPJ Email", "<h1>This is a test sent from a console app</h1>", true);
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

    private static string GetToEmail()
    {
        Console.WriteLine("Enter email address to send to:");
        var toEmail = Console.ReadLine();

        if (!string.IsNullOrEmpty(toEmail)) return toEmail;

        Console.WriteLine();

        return GetToEmail();
    }

}