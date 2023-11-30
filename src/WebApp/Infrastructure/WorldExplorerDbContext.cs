namespace WebApp.Infrastructure;

using System.Reflection.Emit;
using Configurations;
using Entities;
using Microsoft.EntityFrameworkCore;

public class WorldExplorerDbContext : DbContext
{
	public WorldExplorerDbContext(DbContextOptions<WorldExplorerDbContext> options) : base(options)
	{
	}

	public DbSet<Place> Places => Set<Place>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Visit> Visits => Set<Visit>();
	public DbSet<LocationInfoRequest> LocationInfoRequests => Set<LocationInfoRequest>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new PlaceConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new ReviewConfiguration());
		modelBuilder.ApplyConfiguration(new VisitConfiguration());
		modelBuilder.ApplyConfiguration(new LocationInfoRequestConfiguration());
	}
}