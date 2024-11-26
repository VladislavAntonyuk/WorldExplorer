namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Application.Travellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TravellerConfiguration : IEntityTypeConfiguration<Traveller>
{
	public void Configure(EntityTypeBuilder<Traveller> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(t => t.Visits)
		      .WithOne()
		      .HasForeignKey(v => v.TravellerId)
		      .OnDelete(DeleteBehavior.Cascade);
	}
}