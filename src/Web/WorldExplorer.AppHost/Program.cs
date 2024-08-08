var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithImageTag("latest");

var sqlServer = builder.AddSqlServer("server")
                       .WithImage("mssql/server", "2019-CU18-ubuntu-20.04")
                       .WithImageRegistry("mcr.microsoft.com")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("database", "worldexplorer");

var apiService = builder.AddProject<Projects.WorldExplorer_ApiService>("apiservice")
                        .WithReference(cache)
                        .WithReference(sqlServer);

builder.AddProject<Projects.WorldExplorer_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WithReference(apiService);

builder.Build().Run();
