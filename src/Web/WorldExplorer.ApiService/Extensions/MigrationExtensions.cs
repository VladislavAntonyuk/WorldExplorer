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
		if (context is UsersDbContext usersDbContext)
		{
			usersDbContext.Add(User.Create(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6"), new()));
		//	usersDbContext.SaveChanges();
		}
		if (context is PlacesDbContext placesDbContext)
		{
			placesDbContext.Add(WorldExplorer.Modules.Places.Domain.Places.Place.Create("Place1", new(49.419500, 26.995900){SRID = 4326}, "Description1"));
			placesDbContext.Add(WorldExplorer.Modules.Places.Domain.Places.Place.Create("Dnipro", new(48.482, 34.998){SRID = 4326}, "Description2"));


			placesDbContext.Add(new LocationInfoRequest()
			{
				Location = new(49.419500, 26.995900) {SRID = 4326},
				Status = LocationInfoRequestStatus.Completed,
				CreationDate = DateTime.Today
			});

		//	placesDbContext.SaveChanges();
		}
		if (context is TravellersDbContext travellersDbContext)
		{
			travellersDbContext.Add(new Traveller() { Id = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6") });
		//	travellersDbContext.SaveChanges();
		}
	}
}
