namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using System.Data.Common;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public static class AspireOllamaExtensions
{
	public static void AddOllamaApiClient(this IHostApplicationBuilder builder, string connectionName)
	{
		var connectionString = builder.Configuration.GetConnectionString(connectionName);
		if (string.IsNullOrEmpty(connectionString))
		{
			return;
		}

		var parts = connectionString.Split(';').ToDictionary(x => x.Split('=')[0], s => s.Split('=')[1]);

		builder.Services.AddChatClient(chatClientBuilder =>
		{
			var chatClient = chatClientBuilder.UseLogging()
			                                  .UseOpenTelemetry();
			if (!builder.Environment.IsDevelopment())
			{
				chatClient = chatClient.UseDistributedCache();
			}

			return chatClient.Use(new OllamaChatClient(parts["Endpoint"], parts["Model"]));
		});
	}
}