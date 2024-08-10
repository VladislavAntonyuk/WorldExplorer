namespace WorldExplorer.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;
using Modules.Places.Infrastructure.Database;
using WorldExplorer.Modules.Users.Infrastructure.Database;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        ApplyMigration<UsersDbContext>(scope);
        ApplyMigration<PlacesDbContext>(scope);
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.Migrate();
    }
}
