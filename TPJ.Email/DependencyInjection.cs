using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TPJ.Email;

public static class DependencyInjection
{
    public static void AddTPJEmail(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<IEmailer, Emailer>();
    }
}
