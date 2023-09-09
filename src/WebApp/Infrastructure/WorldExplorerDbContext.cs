namespace WebApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Models;

public class WorldExplorerDbContext : DbContext
{
	public WorldExplorerDbContext(DbContextOptions<WorldExplorerDbContext> options) : base(options)
	{
	}

	public DbSet<Place> Places => Set<Place>();
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Place>()
					.Property(e => e.Location)
					.HasColumnType("POINT");
		modelBuilder.Entity<Place>()
					.OwnsMany(post => post.Images, builder => { builder.ToJson(); });
		modelBuilder.Entity<Place>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasMany(x=>x.Reviews)
			      .WithOne().HasForeignKey(d => d.PlaceId)
			      .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.HasMany(x => x.Visits).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Visit>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.HasOne<Place>()
				  .WithMany()
				  .HasForeignKey(d => d.PlaceId)
				  .OnDelete(DeleteBehavior.Cascade); // prevent cascade delete
		});

		modelBuilder.Entity<Review>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.HasOne<User>()
				  .WithMany()
				  .HasForeignKey(d => d.UserId)
				  .OnDelete(DeleteBehavior.Cascade); // On user deletion, associated reviews will be deleted
		});

		modelBuilder.Entity<User>()
		            .HasData(new User
		            {
			            Id = "19d3b2c7-8714-4851-ac73-95aeecfba3a6"
		            });
	}
}