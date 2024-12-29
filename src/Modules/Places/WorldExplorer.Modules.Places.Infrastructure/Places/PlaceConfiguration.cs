namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(place => place.Images)
		       .WithOne()
		       .HasForeignKey(x => x.PlaceId)
		       .OnDelete(DeleteBehavior.Cascade);
	}
}