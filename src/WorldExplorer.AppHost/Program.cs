var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                   .WithImageTag("latest");

var database = builder.AddSqlServer("database")
                      .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                      .PublishAsAzureSqlDatabase();

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(redis)
       .WithReference(database);

builder.Build().Run();
