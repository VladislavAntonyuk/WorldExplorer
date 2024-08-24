using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

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

var apiService = builder.AddProject<Projects.WorldExplorer_ApiService>("apiservice")
						.WithReference(sqlServer)
						.WithReference(openai);

var frontend = builder.AddProject<Projects.WorldExplorer_Web>("webfrontend")
	   .WithExternalHttpEndpoints()
	   .WithReference(apiService);

if (!builder.Environment.IsDevelopment())
{
	var serviceBus = builder.AddRabbitMQ("servicebus")
							.WithImageTag("3.13")
							.WithDataVolume("world-explorer-servicebus");

	var cache = builder.AddRedis("cache")
					   .WithImageTag("latest")
					   .WithDataVolume("world-explorer-cache");

	apiService.WithReference(serviceBus)
			  .WithReference(cache);

	frontend.WithReference(cache);
}

builder.Build().Run();
