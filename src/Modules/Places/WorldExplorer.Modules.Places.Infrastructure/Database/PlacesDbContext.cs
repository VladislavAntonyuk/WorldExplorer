namespace WorldExplorer.Modules.Places.Infrastructure.Database;

using Common.Application.Abstractions.Data;
using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Places;
using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Outbox;

public sealed class PlacesDbContext(DbContextOptions<PlacesDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<Place> Places { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Places);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new PlaceConfiguration());
    }
}
