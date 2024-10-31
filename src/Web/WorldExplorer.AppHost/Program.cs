using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithImageTag("latest").WithDataVolume("world-explorer-cache");

var openai = builder.AddConnectionString("openai");

var sqlServer = builder.AddSqlServer("server");
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
	sqlServer.WithImageTag("2019-CU18-ubuntu-20.04");
}
else
{
	sqlServer.WithImage("azure-sql-edge")
	         .WithImageRegistry("mcr.microsoft.com");
}

sqlServer.WithDataVolume("world-explorer-database")
         .AddDatabase("database", "worldexplorer");

 var aiService = builder.AddOllama("ai")
                        .WithDefaultModel("llama3.2")
                        .WithDataVolume("ollama");

var apiService = builder.AddProject<WorldExplorer_ApiService>("apiservice")
                        .WithReference(sqlServer)
                        .WaitFor(sqlServer)
                        .WithReference(cache)
                        .WaitFor(cache)
                        .WithReference(openai)
                        .WithReference(aiService)
                        .WaitFor(aiService);

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