﻿namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Microsoft.Extensions.Options;
using Quartz;

internal sealed class ConfigurePlacesJob : IConfigureOptions<QuartzOptions>
{
	public void Configure(QuartzOptions options)
	{
		var placeLookupJobName = typeof(PlacesLookupJob).FullName!;

		options.AddJob<PlacesLookupJob>(configure => configure.WithIdentity(placeLookupJobName))
		       .AddTrigger(configure => configure.ForJob(placeLookupJobName)
		                                         .WithSimpleSchedule(schedule =>
			                                                             schedule.WithIntervalInSeconds(15)
				                                                             .RepeatForever()));

		var placeConfigurationJobName = typeof(PlaceDetailsJob).FullName!;

		options.AddJob<PlaceDetailsJob>(configure => configure.WithIdentity(placeConfigurationJobName))
		       .AddTrigger(configure => configure.ForJob(placeConfigurationJobName)
		                                         .WithSimpleSchedule(schedule =>
			                                                             schedule.WithIntervalInSeconds(15)
				                                                             .RepeatForever()));
	}
}