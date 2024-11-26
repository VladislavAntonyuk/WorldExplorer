namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldExplorer.Modules.Travellers.Application.Visits;

public class ReviewsConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasIndex(r => r.VisitId).IsUnique(); // Ensure one review per visit

		builder.HasOne<Visit>()
		       .WithOne(v => v.Review) // Specify the navigation property in Visit
		       .HasForeignKey<Review>(r => r.VisitId)
		       .OnDelete(DeleteBehavior.Cascade);
	}
}