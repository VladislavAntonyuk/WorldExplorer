namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Redis;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly Testcontainers.MsSql.MsSqlContainer dbContainer = new Testcontainers.MsSql.MsSqlBuilder()
	                                                           .WithPassword("mssql-database")
	                                                           .Build();

	private readonly RedisContainer redisContainer = new RedisBuilder().WithImage("redis:latest").Build();

	public async Task InitializeAsync()
	{
		await dbContainer.StartAsync();
		await redisContainer.StartAsync();
	}

	public new async Task DisposeAsync()
	{
		await dbContainer.StopAsync();
		await redisContainer.StopAsync();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		Environment.SetEnvironmentVariable("ConnectionStrings:Database", dbContainer.GetConnectionString());
		Environment.SetEnvironmentVariable("ConnectionStrings:Cache", redisContainer.GetConnectionString());
	}
}