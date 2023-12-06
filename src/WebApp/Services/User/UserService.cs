namespace WebApp.Services.User;

using System.Threading;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

public class UserService(IGraphClientService graphClient, IDbContextFactory<WorldExplorerDbContext> factory) : IUserService
{
	public async Task<List<User>> GetUsers(CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);
		var dbUsers = await dbContext.Users.ToListAsync(cancellationToken);
		var users = new List<User>();
		foreach (var user in dbUsers.Select(ToModel))
		{
			var profile = await graphClient.GetUser(user.Id, cancellationToken);
			if (profile is null)
			{
				continue;
			}

			user.Name = profile.DisplayName ?? string.Empty;
			user.Email = profile.OtherMails.FirstOrDefault(string.Empty);
			users.Add(user);
		}

		return users;
	}

	public async Task<User?> GetUser(string providerId, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(providerId))
		{
			return null;
		}

		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);

		var dbUser = await dbContext.Users
									.Include(user => user.Visits)
									.FirstOrDefaultAsync(u => u.Id == providerId, cancellationToken);
		if (dbUser is null)
		{
			return null;
		}

		var profile = await graphClient.GetUser(providerId, cancellationToken);
		if (profile is null)
		{
			return null;
		}

		return new User
		{
			Id = dbUser.Id,
			Visits = dbUser.Visits.Select(ToModel).ToList(),
			Name = profile.DisplayName ?? string.Empty,
			Email = profile.OtherMails.FirstOrDefault(string.Empty),
			Settings = ToModel(dbUser.Settings),
			Activities =
			[
				new()
				{
					Date = DateTime.Today,
					Steps = 50
				},
				new()
				{
					Date = DateTime.Today.AddDays(-1),
					Steps = 150
				}
			]
		};
	}

	public async Task DeleteUser(string providerId, CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);
		await dbContext.Users.Where(x => x.Id == providerId).ExecuteDeleteAsync(cancellationToken);
		await graphClient.DeleteAsync(providerId, cancellationToken);
	}

	public async Task UpdateUser(User user, CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);
		var dbUser = await dbContext.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
		if (dbUser is not null)
		{
			dbUser.Settings = ToEntity(user.Settings);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}

	private static User ToModel(Infrastructure.Entities.User user)
	{
		return new User
		{
			Id = user.Id,
			Name = string.Empty,
			Email = string.Empty
		};
	}

	private static Visit ToModel(Infrastructure.Entities.Visit visit)
	{
		return new Visit
		{
			Id = visit.Id,
			Place = new Place
			{
				Id = visit.PlaceId,
				Name = string.Empty,
				Location = new Location(0, 0)
			},
			VisitDate = visit.VisitDate
		};
	}

	private static UserSettings ToModel(Infrastructure.Entities.UserSettings settings)
	{
		return new UserSettings()
		{
			TrackUserLocation = settings.TrackUserLocation
		};
	}

	private static Infrastructure.Entities.UserSettings ToEntity(UserSettings settings)
	{
		return new Infrastructure.Entities.UserSettings()
		{
			TrackUserLocation = settings.TrackUserLocation
		};
	}
}