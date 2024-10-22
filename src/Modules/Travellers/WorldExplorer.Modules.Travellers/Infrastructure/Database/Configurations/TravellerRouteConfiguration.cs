namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Application.Travellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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