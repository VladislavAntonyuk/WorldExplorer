namespace WorldExplorer.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;
using Modules.Places.Domain.LocationInfo;
using Modules.Places.Infrastructure.Database;
using Modules.Travellers;
using Modules.Travellers.Application.Travellers;
using Modules.Users.Domain.Users;
using WorldExplorer.Modules.Travellers.Infrastructure.Database;
using WorldExplorer.Modules.Users.Infrastructure.Database;

public static class MigrationExtensions
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using IServiceScope scope = app.ApplicationServices.CreateScope();

		ApplyMigration<UsersDbContext>(scope);
		ApplyMigration<PlacesDbContext>(scope);
		ApplyMigration<TravellersDbContext>(scope);
	}

	private static void ApplyMigration<TDbContext>(IServiceScope scope)
		where TDbContext : DbContext
	{
		using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		context.Database.Migrate();
	}
}
