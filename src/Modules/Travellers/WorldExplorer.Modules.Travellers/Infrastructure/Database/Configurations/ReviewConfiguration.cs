namespace WebApp.Infrastructure.Configurations;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> builder)
	{
		builder.HasKey(e => e.Id);

		builder.HasOne<User>()
			   .WithMany()
			   .HasForeignKey(d => d.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}