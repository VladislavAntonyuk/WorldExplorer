var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddConnectionString("openai");
//var b = builder.AddAzureAppConfiguration("appconfiguration");


var cache = builder.AddRedis("cache")
                   .WithImageTag("latest");

var sqlServer = builder.AddSqlServer("server")
                         .WithImage("azure-sql-edge")
                         .WithImageRegistry("mcr.microsoft.com")
                         // .WithEnvironment("MSSQL_SA_PASSWORD", "reallyStrongPwd123")
                          .WithEnvironment("ACCEPT_EULA", "1")
                          .WithEnvironment("MSSQL_PID", "Developer")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("database", "worldexplorer");

// var travellerService = builder.AddFusionGateway<Projects.WorldExplorer_Modules_Travellers>("graphql")
//                            .WithReference(sqlServer);

var apiService = builder.AddProject<Projects.WorldExplorer_ApiService>("apiservice")
                        .WithReference(cache)
                        .WithReference(sqlServer)
                        .WithReference(openai);

builder.AddProject<Projects.WorldExplorer_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WithReference(apiService);

builder.Build().Run();
