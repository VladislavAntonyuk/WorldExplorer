namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(post => post.Images)
		       .WithOne()
		       .HasForeignKey(x => x.PlaceId)
		       .OnDelete(DeleteBehavior.Cascade);

		//.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}
public class PlaceImageConfiguration : IEntityTypeConfiguration<PlaceImage>
{
	public void Configure(EntityTypeBuilder<PlaceImage> builder)
	{
		builder.HasKey(e => e.Id);
	}
}