namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldExplorer.Modules.Travellers.Application.Visits;

public class PlacesConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);
		builder.HasMany(p => p.Visits)
		      .WithOne()
		      .HasForeignKey(v => v.PlaceId)
		      .OnDelete(DeleteBehavior.Cascade);
	}
}