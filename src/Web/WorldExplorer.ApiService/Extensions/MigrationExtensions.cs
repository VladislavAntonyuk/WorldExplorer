namespace WorldExplorer.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;
using Modules.Places.Infrastructure.Database;
using Modules.Travellers.Infrastructure.Database;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Database;

public static class MigrationExtensions
{
	public static async Task ApplyMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();

		await ApplyMigration<TravellersDbContext>(scope);
		await ApplyMigration<PlacesDbContext>(scope);
		await ApplyMigration<UsersDbContext>(scope);
	}

	private static async Task ApplyMigration<TDbContext>(IServiceScope scope) where TDbContext : DbContext
	{
		await using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		await context.Database.MigrateAsync();
	}
}