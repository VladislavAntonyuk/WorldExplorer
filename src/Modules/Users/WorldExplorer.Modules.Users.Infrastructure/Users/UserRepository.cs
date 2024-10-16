namespace WorldExplorer.Modules.Users.Infrastructure.Users;

using Database;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

internal sealed class UserRepository(UsersDbContext context) : IUserRepository
{
	public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<List<User>> GetAsync(CancellationToken cancellationToken = default)
	{
		return await context.Users.ToListAsync(cancellationToken);
	}

	public void Insert(User user)
	{
		context.Users.Add(user);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		await context.Users.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
	}

	public void Delete(User user)
	{
		context.Users.Remove(user);
	}
}