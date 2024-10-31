namespace WorldExplorer.Common.Infrastructure.Inbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
	public void Configure(EntityTypeBuilder<InboxMessage> builder)
	{
		builder.ToTable("inbox_messages");

		builder.HasKey(o => o.Id);
	}
}