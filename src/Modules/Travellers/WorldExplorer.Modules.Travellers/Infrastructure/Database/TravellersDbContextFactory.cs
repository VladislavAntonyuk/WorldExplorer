#if DEBUG
namespace WorldExplorer.Modules.Travellers.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// dotnet ef migrations add "Travellers" -o "Infrastructure\Database\Migrations"
public class TravellersDbContextFactory : IDesignTimeDbContextFactory<TravellersDbContext>
{
	public TravellersDbContext CreateDbContext(string[] args)
	{
		return new TravellersDbContext(new DbContextOptionsBuilder<TravellersDbContext>()
		                               .UseSqlServer(
			                               "Host=localhost;Database=worldexplorer;Username=sa;Password=password")
		                               .Options);
	}
}
#endif