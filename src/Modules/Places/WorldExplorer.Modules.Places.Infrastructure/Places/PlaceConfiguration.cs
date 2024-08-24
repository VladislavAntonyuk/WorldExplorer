namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);

		builder.OwnsMany(post => post.Images, x => { x.ToJson(); });

		builder.HasData(
			new { Id = Guid.CreateVersion7(), Name= "Place1", Location = new Point(49.419500, 26.995900) { SRID = 4326 }, Description = "Description1" },
			new { Id = Guid.CreateVersion7(), Name= "Dnipro", Location = new Point(48.482, 34.998) { SRID = 4326 }, Description = "Description2" }
		);

			   //.HasColumnType(Geometry.TypeNamePoint);
		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}