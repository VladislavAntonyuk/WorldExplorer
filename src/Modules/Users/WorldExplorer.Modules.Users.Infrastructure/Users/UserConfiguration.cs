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
	}
}