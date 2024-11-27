namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using AutoFixture;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
	protected static readonly Fixture Faker = new();
	protected readonly UsersDbContext DbContext;
	protected readonly HttpClient HttpClient;
	private readonly IServiceScope scope;
	protected readonly ISender Sender;

	protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
	{
		scope = factory.Services.CreateScope();
		HttpClient = factory.CreateClient();
		Sender = scope.ServiceProvider.GetRequiredService<ISender>();
		DbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
	}

	public void Dispose()
	{
		scope.Dispose();
	}

	protected async Task CleanDatabaseAsync()
	{
		await DbContext.Database.ExecuteSqlRawAsync("""
		                                            DELETE FROM users.inbox_message_consumers;
		                                            DELETE FROM users.inbox_messages;
		                                            DELETE FROM users.outbox_message_consumers;
		                                            DELETE FROM users.outbox_messages;
		                                            DELETE FROM users.users;
		                                            """);
	}
}