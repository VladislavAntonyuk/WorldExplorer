namespace WebApp.Infrastructure.Configurations;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Place;

public class LocationInfoRequestConfiguration : IEntityTypeConfiguration<LocationInfoRequest>
{
	public void Configure(EntityTypeBuilder<LocationInfoRequest> builder)
	{
		
	}
}