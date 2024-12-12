namespace WorldExplorer.Modules.Places.Infrastructure.LocationInfo;

using Domain.LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LocationInfoRequestConfiguration : IEntityTypeConfiguration<LocationInfoRequest>
{
	public void Configure(EntityTypeBuilder<LocationInfoRequest> builder)
	{
		builder.HasKey(e => e.Id);
	}
}