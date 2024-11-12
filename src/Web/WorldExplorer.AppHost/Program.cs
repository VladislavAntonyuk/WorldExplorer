using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithImageTag("latest").WithDataVolume("world-explorer-cache");

var sqlServer = builder.AddSqlServer("sqlserver")
					   .WithDataVolume("world-explorer-database");
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
	sqlServer.WithImage("azure-sql-edge")
			 .WithImageRegistry("mcr.microsoft.com");
}

var database = sqlServer
			   .AddDatabase("database", "worldexplorer");

var apiService = builder.AddProject<WorldExplorer_ApiService>("apiservice")
						.WithReference(sqlServer)
						.WaitFor(database)
						.WithReference(cache)
						.WaitFor(cache);

if (builder.Environment.IsDevelopment())
{
	var aiService = builder.AddOllama("ai")
						   .WithDataVolume("ollama")
						   .WithOpenWebUI();
	var llama = aiService.AddModel("llama3.2:latest");
	var phi = aiService.AddModel("phi3.5:latest");

	apiService.WithReference(phi)
			  .WaitFor(phi);
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
	var serviceBus = builder.AddRabbitMQ("servicebus").WithImageTag("3.13").WithDataVolume("world-explorer-servicebus");

	apiService.WithReference(serviceBus);
}

builder.AddMobileProject("mauiclient", "../../Client/Client", clientStubProjectPath: "../../Client/Client.ClientStub/Client.ClientStub.csproj")
	   .WithReference(apiService)
	   .WaitFor(apiService);

await builder.Build().RunAsync();