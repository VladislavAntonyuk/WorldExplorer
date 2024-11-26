namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldExplorer.Modules.Travellers.Application.Visits;

public class VisitsConfiguration : IEntityTypeConfiguration<Visit>
{
	public void Configure(EntityTypeBuilder<Visit> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasIndex(v => new { v.TravellerId, v.PlaceId }).IsUnique();
	}
}