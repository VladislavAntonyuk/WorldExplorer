namespace WebApp.Infrastructure.Configurations;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Place;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
	public void Configure(EntityTypeBuilder<Place> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Location)
			   .HasSrid(DistanceConstants.SRID)
			   .HasColumnType("POINT");

		builder.OwnsMany(post => post.Images, x => { x.ToJson(); });

		builder.HasMany(x => x.Reviews)
			   .WithOne().HasForeignKey(d => d.PlaceId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}