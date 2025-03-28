﻿namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using Microsoft.Extensions.Options;
using Quartz;

internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
	public void Configure(QuartzOptions options)
	{
		var jobName = typeof(ProcessOutboxJob).FullName!;

		options.AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
			   .AddTrigger(configure =>
			   {
				   configure.ForJob(jobName)
				            .WithSimpleSchedule(schedule =>
				            {
					            schedule
						            .WithIntervalInSeconds(outboxOptions.Value.IntervalInSeconds)
						            .RepeatForever();
				            });
			   });
	}
}