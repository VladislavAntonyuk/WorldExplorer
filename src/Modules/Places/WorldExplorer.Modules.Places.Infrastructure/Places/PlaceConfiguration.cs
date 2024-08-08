namespace WorldExplorer.Modules.Places.Infrastructure.Places;

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

		builder.Property(x => x.Location)
			   .HasConversion(l =>
								  Geometry.DefaultFactory.CreatePoint(new Coordinate(l.Longitude, l.Latitude)),
			                  p => new Location(p.X, p.Y))
			   .HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}