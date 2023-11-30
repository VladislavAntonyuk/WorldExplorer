namespace WebApp.Infrastructure.Configurations;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
	public void Configure(EntityTypeBuilder<Visit> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasOne<Place>()
			   .WithMany()
			   .HasForeignKey(d => d.PlaceId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}