using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithImageTag("latest").WithDataVolume("world-explorer-cache");

var openai = builder.AddConnectionString("openai");

var sqlServer = builder.AddSqlServer("server")
                       //.WithImage("azure-sql-edge")
                       //.WithImageRegistry("mcr.microsoft.com")
                       .WithImageTag("2019-CU18-ubuntu-20.04")
                       .WithDataVolume("world-explorer-database")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("database", "worldexplorer");

// var travellerService = builder.AddFusionGateway<Projects.WorldExplorer_Modules_Travellers>("graphql")
//                            .WithReference(sqlServer);

var apiService = builder.AddProject<WorldExplorer_ApiService>("apiservice")
                        .WithReference(sqlServer)
                        .WithReference(cache)
                        .WithReference(openai);

builder.AddProject<WorldExplorer_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(apiService)
       .WithReference(cache);

if (!builder.Environment.IsDevelopment())
{
	var serviceBus = builder.AddRabbitMQ("servicebus").WithImageTag("3.13").WithDataVolume("world-explorer-servicebus");

	apiService.WithReference(serviceBus);
}

builder.AddMobileProject("mauiclient", "../../Client/Client")
       .WithReference(apiService);

builder.Build().Run();