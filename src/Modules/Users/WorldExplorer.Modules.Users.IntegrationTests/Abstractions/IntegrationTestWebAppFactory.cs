﻿namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using Fixtures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Xunit;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly MsSqlContainer dbContainer = new MsSqlBuilder()
	                                              .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
	                                              .Build();

	private readonly RedisContainer redisContainer = new RedisBuilder().WithImage("redis:latest").Build();

	public async ValueTask InitializeAsync()
	{
		await dbContainer.StartAsync();
		await redisContainer.StartAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await dbContainer.StopAsync();
		await redisContainer.StopAsync();
		await base.DisposeAsync();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		Environment.SetEnvironmentVariable("ConnectionStrings:database", dbContainer.GetConnectionString());
		Environment.SetEnvironmentVariable("ConnectionStrings:cache", redisContainer.GetConnectionString());
		Environment.SetEnvironmentVariable("ConnectionStrings:ai-llama3-2", "Endpoint=https://localhost;Model=llama3.2");

		builder.ConfigureServices(services =>
		{
			services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
			services.Configure<TestAuthHandlerOptions>(options =>
			{
				options.FakeSuccessfulAuthentication = true;
			});
		});
	}
}