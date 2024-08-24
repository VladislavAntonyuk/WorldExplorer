namespace WorldExplorer.ApiService.Extensions;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modules.Places.Infrastructure.Database;
using Modules.Travellers.Infrastructure.Database;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Database;

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

		// todo remove
		if (context is UsersDbContext usersDbContext)
		{
			var user = usersDbContext.Find<User>(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6"));
			if (user != null)
			{
				return;
			}

			usersDbContext.Add(User.Create(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6"), new UserSettings()));
			usersDbContext.SaveChanges();
		}
	}
}
