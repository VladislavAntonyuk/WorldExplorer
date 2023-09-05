namespace WebApp.Services;

using global::Shared.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;
using Visit = Infrastructure.Models.Visit;

public class UserService : IUserService
{
	private readonly GraphServiceClient graphClient;
	private readonly IDbContextFactory<WorldExplorerDbContext> factory;

	public UserService(GraphServiceClient graphClient, IDbContextFactory<WorldExplorerDbContext> factory)
	{
		this.graphClient = graphClient;
		this.factory = factory;
	}

	public async Task<User?> GetUser(string providerId, CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);

		var dbUser = await dbContext.Users
		                            .Include(x => x.Visits)
		                            .FirstOrDefaultAsync(x => x.Id == providerId, cancellationToken);
		if (dbUser is null)
		{
			return null;
		}

		return new User
		{
			Id = dbUser.Id,
			Visits = dbUser.Visits.Select(ToDto).ToList(),
			Name = string.Empty,
			Email = string.Empty
		};
	}

	private global::Shared.Models.Visit ToDto(Visit arg)
	{
		return new global::Shared.Models.Visit()
		{
			Id = arg.Id,
			Place = new Place
			{
				Id = arg.PlaceId,
				Name = string.Empty,
				Location = new Location(0, 0)
			},
			VisitDate = arg.VisitDate
		};
	}

	public async Task DeleteUser(string providerId, CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);
		await dbContext.Users.Where(x => x.Id == providerId).ExecuteDeleteAsync(cancellationToken);
		await graphClient.Users[providerId].DeleteAsync(cancellationToken: cancellationToken);
	}
}