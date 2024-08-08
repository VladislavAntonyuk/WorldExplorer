using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace WorldExplorer.Modules.Users.Infrastructure.Database;

using Common.Application.Abstractions.Data;
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
