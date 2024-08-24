namespace WorldExplorer.Modules.Users.Infrastructure.Users;

using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(u => u.Id);
		builder.OwnsOne(u => u.Settings, x => { x.ToJson(); });

		builder.HasData(new
		{
			Id = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6"),
		});
	}
}
