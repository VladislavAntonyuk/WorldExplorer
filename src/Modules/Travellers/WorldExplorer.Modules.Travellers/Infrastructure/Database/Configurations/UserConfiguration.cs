namespace WebApp.Infrastructure.Configurations;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(e => e.Id);
		builder.HasMany(x => x.Visits).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
		builder.OwnsOne(post => post.Settings, x => { x.ToJson(); });
	}
}