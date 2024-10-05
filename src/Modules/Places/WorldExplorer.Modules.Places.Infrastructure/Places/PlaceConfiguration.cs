namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.OwnsMany(post => post.Images, x => { x.ToJson(); });

		//.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}