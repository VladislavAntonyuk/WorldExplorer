using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
				   .WithRedisInsight(s => s.WithLifetime(ContainerLifetime.Persistent))
				   .WithLifetime(ContainerLifetime.Persistent)
				   .WithDataVolume("world-explorer-cache");

var sqlServer = builder.AddSqlServer("sqlserver")
					   .WithLifetime(ContainerLifetime.Persistent)
					   .WithDataVolume("world-explorer-database");

var database = sqlServer.AddDatabase("database", "worldexplorer");

var apiService = builder.AddProject<WorldExplorer_ApiService>("apiservice")
						.WithReference(sqlServer)
						.WaitFor(database)
						.WithReference(cache)
						.WaitFor(cache);

if (builder.Environment.IsDevelopment())
{
	var aiService = builder.AddOllama("ai")
						   .WithLifetime(ContainerLifetime.Persistent)
						   .WithDataVolume("ollama")
						   .WithOpenWebUI(s => s.WithLifetime(ContainerLifetime.Persistent))
						   .AddModel("llama3.2:latest");// phi3.5:latest // llama3.2:latest

	apiService.WithReference(aiService)
			  .WaitFor(aiService);
}
else
{
	var openai = builder.AddConnectionString("openai");
	apiService.WithReference(openai);
}

builder.AddProject<WorldExplorer_Web>("webfrontend")
	   .WithExternalHttpEndpoints()
	   .WithReference(apiService)
	   .WaitFor(apiService)
	   .WithReference(cache)
	   .WaitFor(cache);

if (!builder.Environment.IsDevelopment())
{
	var serviceBus = builder.AddRabbitMQ("servicebus")
							.WithManagementPlugin()
							.WithLifetime(ContainerLifetime.Persistent)
							.WithDataVolume("world-explorer-servicebus");

	apiService.WithReference(serviceBus);
}

await builder.Build().RunAsync();