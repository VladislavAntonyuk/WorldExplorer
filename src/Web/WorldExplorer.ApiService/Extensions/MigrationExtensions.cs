namespace WorldExplorer.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;
using Modules.Places.Infrastructure.Database;
using Modules.Travellers.Infrastructure.Database;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Database;

public static class MigrationExtensions
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();

		ApplyMigration<UsersDbContext>(scope);
		ApplyMigration<PlacesDbContext>(scope);
		ApplyMigration<TravellersDbContext>(scope);
	}

	private static void ApplyMigration<TDbContext>(IServiceScope scope) where TDbContext : DbContext
	{
		using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		context.Database.Migrate();
	}
}