namespace WorldExplorer.Modules.Travellers.Database;

using Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Outbox;

public sealed class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Travellers);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }

    internal DbSet<Traveller> Travellers { get; set; }
    internal DbSet<Visit> Visits { get; set; }
    internal DbSet<Place> Places { get; set; }
}
