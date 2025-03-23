namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using AutoFixture;
using Fixtures;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
	protected static readonly Fixture Faker = new();
	protected readonly UsersDbContext DbContext;
	protected readonly HttpClient HttpClient;
	private readonly IServiceScope scope;
	protected readonly ISender Sender;
	private bool disposedValue;

	protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
	{
		scope = factory.Services.CreateScope();
		HttpClient = factory.CreateClient();
		Sender = scope.ServiceProvider.GetRequiredService<ISender>();
		DbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
	}

	protected void SetAuth(bool enableAuth, bool failPermission = false)
	{
		var testAuthHandlerOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<TestAuthHandlerOptions>>();
		testAuthHandlerOptions.CurrentValue.FakeSuccessfulAuthentication = enableAuth;
		testAuthHandlerOptions.CurrentValue.FailPermission = failPermission;
	}

	protected async Task CleanDatabaseAsync()
	{
		await DbContext.Database.ExecuteSqlRawAsync("""
		                                            DELETE FROM users.inbox_messages;
		                                            DELETE FROM users.outbox_messages;
		                                            DELETE FROM users.users;
		                                            """);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				scope.Dispose();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}