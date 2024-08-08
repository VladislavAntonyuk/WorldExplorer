using Microsoft.Extensions.Configuration;

namespace WorldExplorer.Common.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ??
               throw new InvalidOperationException($"The connection string {name} was not found");
    }

    public static T GetValueOrThrow<T>(this IConfiguration configuration, string name)
    {
        return configuration.GetValue<T?>(name) ??
               throw new InvalidOperationException($"The connection string {name} was not found");
    }
}
