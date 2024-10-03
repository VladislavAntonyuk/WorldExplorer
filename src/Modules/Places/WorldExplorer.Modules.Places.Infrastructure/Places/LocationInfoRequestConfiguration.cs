namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.LocationInfo;
using LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

public class LocationInfoRequestConfiguration : IEntityTypeConfiguration<LocationInfoRequest>
{
	public void Configure(EntityTypeBuilder<LocationInfoRequest> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasData(new
		{
			Id = 1,
			Location = new Point(26.995900, 49.419500)
			{
				SRID = DistanceConstants.SRID
			},
			Status = LocationInfoRequestStatus.Completed,
			CreationDate = DateTime.Today
		});
		//.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}