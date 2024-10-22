namespace WorldExplorer.Modules.Travellers.Infrastructure.Database;

using Abstractions.Data;
using Application.Travellers;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

public sealed class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options), IUnitOfWork
{
	internal DbSet<Traveller> Travellers => Set<Traveller>();
	internal DbSet<Visit> Visits => Set<Visit>();
	internal DbSet<Place> Places => Set<Place>();

	internal DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
	internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
	internal DbSet<InboxMessageConsumer> InboxMessagesConsumers => Set<InboxMessageConsumer>();
	internal DbSet<OutboxMessageConsumer> OutboxMessagesConsumers => Set<OutboxMessageConsumer>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(Schemas.Travellers);

		modelBuilder.ApplyConfiguration(new TravellerConfiguration());
		modelBuilder.ApplyConfiguration(new TravellerRouteConfiguration());

		modelBuilder.ApplyConfiguration(new PlacesConfiguration());
		modelBuilder.ApplyConfiguration(new VisitsConfiguration());
		modelBuilder.ApplyConfiguration(new ReviewsConfiguration());

		modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
	}
}