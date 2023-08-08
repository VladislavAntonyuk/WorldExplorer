namespace WebApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Models;

public class WorldExplorerDbContext : DbContext
{
	public WorldExplorerDbContext(DbContextOptions<WorldExplorerDbContext> options) : base(options)
	{
	}

	public DbSet<Place> Places => Set<Place>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Place>()
					.OwnsOne(post => post.Location, builder => { builder.ToJson(); })
					.OwnsMany(post => post.Images, builder => { builder.ToJson(); });
	}
}