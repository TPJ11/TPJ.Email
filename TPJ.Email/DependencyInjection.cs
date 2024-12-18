using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TPJ.Email;

public static class DependencyInjection
{
    public static void AddTPJEmail(this IServiceCollection services)
    {
        services.TryAddSingleton<IEmailSettings, EmailSettings>();
        services.TryAddSingleton<IEmailer, Emailer>();
    }
}
