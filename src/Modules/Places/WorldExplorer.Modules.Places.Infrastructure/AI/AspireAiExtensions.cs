namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;

public static class AspireAiExtensions
{
	public static void AddOpenAiClient(this IHostApplicationBuilder builder, string model)
	{
		builder.AddChatClient(sp => new OpenAIChatClient(sp.GetRequiredService<OpenAIClient>(), model));
	}

	public static void AddOllamaChatClient(this IHostApplicationBuilder builder, string connectionName)
	{
		var connectionString = builder.Configuration.GetConnectionString(connectionName);
		if (string.IsNullOrEmpty(connectionString))
		{
			return;
		}

		var parts = connectionString.Split(';').ToDictionary(x => x.Split('=')[0], s => s.Split('=')[1]);
		builder.AddChatClient(_ => new OllamaChatClient(parts["Endpoint"], parts["Model"]));
	}

	public static void AddChatClient(this IHostApplicationBuilder builder, Func<IServiceProvider, IChatClient> innerClient)
	{
		builder.Services.AddChatClient(sp =>
		{
			var chatClientBuilder = new ChatClientBuilder(innerClient)
							 .UseLogging()
							 .UseOpenTelemetry();
			if (!builder.Environment.IsDevelopment())
			{
				chatClientBuilder = chatClientBuilder.UseDistributedCache();
			}

			return chatClientBuilder.Build(sp);
		});
	}
}