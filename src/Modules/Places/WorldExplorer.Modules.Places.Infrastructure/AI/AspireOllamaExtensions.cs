namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

		builder.Services.AddChatClient(sp =>
		{
			var chatClient = new ChatClientBuilder(new OllamaChatClient(parts["Endpoint"], parts["Model"]))
			                 .UseLogging(sp.GetRequiredService<ILoggerFactory>())
			                 .UseFunctionInvocation(sp.GetRequiredService<ILoggerFactory>(), client => client.DetailedErrors = true)
							 .UseOpenTelemetry(sp.GetRequiredService<ILoggerFactory>(), "ollama");
			if (!builder.Environment.IsDevelopment())
			{
				chatClient = chatClient.UseDistributedCache();
			}

			return chatClient.Build(sp);
		});
	}
}