var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddConnectionString("openai");
//var b = builder.AddAzureAppConfiguration("appconfiguration");

var reviewService = builder.AddFusionGateway<Projects.WorldExplorer_Modules_Travellers>("graphql");

var cache = builder.AddRedis("cache")
                   .WithImageTag("latest");

var sqlServer = builder.AddSqlServer("server")
                        .WithImage("azure-sql-edge")
                        .WithImageRegistry("mcr.microsoft.com")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("database", "worldexplorer");

var apiService = builder.AddProject<Projects.WorldExplorer_ApiService>("apiservice")
                        .WithReference(reviewService)
                        .WithReference(cache)
                        .WithReference(sqlServer)
                        .WithReference(openai);

builder.AddProject<Projects.WorldExplorer_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WithReference(apiService);

builder.Build().Run();
