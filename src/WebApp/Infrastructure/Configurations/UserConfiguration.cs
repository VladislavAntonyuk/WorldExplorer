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

		builder.HasData(new User
		{
			Id = "19d3b2c7-8714-4851-ac73-95aeecfba3a6"
		});
	}
}