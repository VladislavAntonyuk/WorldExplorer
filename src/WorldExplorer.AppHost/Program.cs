var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
				   .WithImageTag("latest");

var sqlServer = builder.AddSqlServer("server")
                       .WithImage("mssql/server", "2019-CU18-ubuntu-20.04")
                       .WithImageRegistry("mcr.microsoft.com")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("database", "worldexplorer");

builder.AddProject<Projects.WebApp>("webapp")
	   .WithReference(redis)
	   .WithReference(sqlServer);

builder.Build().Run();
