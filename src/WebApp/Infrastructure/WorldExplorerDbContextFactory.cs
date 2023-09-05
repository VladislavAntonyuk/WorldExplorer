#if DEBUG
namespace WebApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class WorldExplorerDbContextFactory : IDesignTimeDbContextFactory<WorldExplorerDbContext>
{
	public WorldExplorerDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<WorldExplorerDbContext>();
		const string connectionString = "Data Source=migration.db";
		optionsBuilder.UseSqlite(connectionString, options => options.UseNetTopologySuite());
		return new WorldExplorerDbContext(optionsBuilder.Options);
	}
}
#endif