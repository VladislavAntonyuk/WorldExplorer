namespace WebApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Models;

#if DEBUG
public class WorldExplorerDbContextFactory : IDesignTimeDbContextFactory<WorldExplorerDbContext>
{
	public WorldExplorerDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<WorldExplorerDbContext>();
		const string connectionString = "Data Source=migration.db";
		optionsBuilder.UseSqlite(connectionString);
		return new WorldExplorerDbContext(optionsBuilder.Options);
	}
}
#endif

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