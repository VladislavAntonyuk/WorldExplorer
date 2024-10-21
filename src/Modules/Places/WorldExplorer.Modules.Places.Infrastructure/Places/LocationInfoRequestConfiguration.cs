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

		//.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}