namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using WorldExplorer.Modules.Places.Domain.Places;
using Location = Domain.Places.Location;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.OwnsMany(post => post.Images, x => { x.ToJson(); });

			   //.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}
public class LocationInfoRequestConfiguration : IEntityTypeConfiguration<LocationInfoRequest>
{
	public void Configure(EntityTypeBuilder<LocationInfoRequest> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(x => x.Location)
			   .HasConversion(l =>
								  new Point(l.Longitude, l.Latitude)
								  {
									  SRID = 4326
								  },
							  p => new Location(p.X, p.Y));
			   //.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}