namespace WorldExplorer.Common.Infrastructure.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
	public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
	{
		builder.ToTable("outbox_message_consumers");

		builder.HasKey(o => new
		{
			o.OutboxMessageId,
			o.Name
		});

		builder.Property(o => o.Name).HasMaxLength(500);
	}
}