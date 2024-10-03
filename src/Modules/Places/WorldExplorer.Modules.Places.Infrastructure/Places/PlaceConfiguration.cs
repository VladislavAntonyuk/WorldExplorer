namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.OwnsMany(post => post.Images, x => { x.ToJson(); });

		builder.HasData(new
		{
			Id = Guid.CreateVersion7(),
			Name = "Place1",
			Location = new Point(26.995900, 49.419500)
			{
				SRID = DistanceConstants.SRID
			},
			Description = "Description1"
		}, new
		{
			Id = Guid.CreateVersion7(),
			Name = "Dnipro",
			Location = new Point(34.998, 48.482)
			{
				SRID = DistanceConstants.SRID
			},
			Description = "Description2"
		});

		//.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}