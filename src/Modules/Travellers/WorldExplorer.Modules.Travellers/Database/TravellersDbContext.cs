namespace WorldExplorer.Modules.Travellers.Database;

using Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Outbox;

public sealed class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Travellers);

		modelBuilder.ApplyConfiguration(new TravellerConfiguration());
		modelBuilder.ApplyConfiguration(new TravellerRouteConfiguration());
		modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }

    internal DbSet<Traveller> Travellers { get; set; }
    //internal DbSet<Visit> Visits { get; set; }
    //internal DbSet<Place> Places { get; set; }
}


#if DEBUG
// dotnet ef migrations add "Travellers" -o "Database\Migrations"
public class TravellersDbContextFactory : IDesignTimeDbContextFactory<TravellersDbContext>
{
	public TravellersDbContext CreateDbContext(string[] args)
	{
		return new TravellersDbContext(new DbContextOptionsBuilder<TravellersDbContext>()
		                               .UseSqlServer("Host=localhost;Database=worldexplorer;Username=sa;Password=password")
		                               .Options);
	}
}
#endif

public class TravellerConfiguration : IEntityTypeConfiguration<Traveller>
{
	public void Configure(EntityTypeBuilder<Traveller> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(x => x.Routes).WithOne();

		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}

public class TravellerRouteConfiguration : IEntityTypeConfiguration<TravellerRoute>
{
	public void Configure(EntityTypeBuilder<TravellerRoute> builder)
	{
		builder.HasKey(e => e.Id);

		builder.OwnsMany(post => post.Locations, x => { x.ToJson(); });

		//builder.Property(x => x.Location)
		//       .HasConversion(l =>
		//	                      Geometry.DefaultFactory.CreatePoint(new Coordinate(l.Longitude, l.Latitude)),
		//                      p => new Location(p.X, p.Y))
		//       .HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}