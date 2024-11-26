namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;

public static class AspireAiExtensions
{
	public static void AddOpenAiClient(this IHostApplicationBuilder builder, string connectionName)
	{
		var connectionString = builder.Configuration.GetConnectionString(connectionName);
		if (string.IsNullOrEmpty(connectionString))
		{
			return;
		}

		var parts = connectionString.Split(';').ToDictionary(x => x.Split('=')[0], s => s.Split('=')[1]);
		var client = new OpenAIClient(new ApiKeyCredential(parts["Key"]), new OpenAIClientOptions
		{
			Endpoint = new Uri(parts["Endpoint"])
		});
		builder.AddChatClient(new OpenAIChatClient(client, "gpt-4o-mini"));
	}

	public static void AddOllamaChatClient(this IHostApplicationBuilder builder, string connectionName)
	{
		var connectionString = builder.Configuration.GetConnectionString(connectionName);
		if (string.IsNullOrEmpty(connectionString))
		{
			return;
		}

		var parts = connectionString.Split(';').ToDictionary(x => x.Split('=')[0], s => s.Split('=')[1]);
		builder.AddChatClient(new OllamaChatClient(parts["Endpoint"], parts["Model"]));
	}

	public static void AddChatClient(this IHostApplicationBuilder builder, IChatClient innerClient)
	{
		builder.Services.AddChatClient(sp =>
		{
			var chatClient = new ChatClientBuilder(innerClient)
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