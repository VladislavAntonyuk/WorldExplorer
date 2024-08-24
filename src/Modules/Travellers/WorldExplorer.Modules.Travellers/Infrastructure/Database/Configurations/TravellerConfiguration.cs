namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Application.Travellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TravellerConfiguration : IEntityTypeConfiguration<Traveller>
{
	public void Configure(EntityTypeBuilder<Traveller> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasMany(x => x.Routes).WithOne();

		builder.HasData(new Traveller() { Id = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6") });

		//builder.HasMany(x => x.Reviews)
		//	   .WithOne().HasForeignKey(d => d.PlaceId)
		//	   .OnDelete(DeleteBehavior.Cascade);
	}
}