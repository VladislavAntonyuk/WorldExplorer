namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PlaceImageConfiguration : IEntityTypeConfiguration<PlaceImage>
{
	public void Configure(EntityTypeBuilder<PlaceImage> builder)
	{
		builder.HasKey(e => e.Id);
	}
}