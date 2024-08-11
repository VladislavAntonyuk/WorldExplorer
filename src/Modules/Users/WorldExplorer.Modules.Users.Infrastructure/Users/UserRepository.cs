using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace WorldExplorer.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
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
}
