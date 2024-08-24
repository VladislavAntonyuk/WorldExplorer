namespace WorldExplorer.Modules.Users.Infrastructure.Database;

using Common.Application.Abstractions.Data;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Users;
using User = Domain.Users.User;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}

#if DEBUG
// dotnet ef migrations add "Users" -o "Database\Migrations"
public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
	public UsersDbContext CreateDbContext(string[] args)
	{
		return new UsersDbContext(new DbContextOptionsBuilder<UsersDbContext>()
			.UseSqlServer("Host=localhost;Database=worldexplorer;Username=sa;Password=password")
			.Options);
	}
}
#endif