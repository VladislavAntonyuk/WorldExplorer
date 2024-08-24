namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
	protected static readonly Fixture Faker = new();
	private readonly IServiceScope _scope;
	protected readonly UsersDbContext DbContext;
	protected readonly HttpClient HttpClient;
	protected readonly ISender Sender;

	protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
	{
		_scope = factory.Services.CreateScope();
		HttpClient = factory.CreateClient();
		Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
		DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
	}

	public void Dispose()
	{
		_scope.Dispose();
	}

	protected async Task CleanDatabaseAsync()
	{
		await DbContext.Database.ExecuteSqlRawAsync("""
		                                            DELETE FROM users.inbox_message_consumers;
		                                            DELETE FROM users.inbox_messages;
		                                            DELETE FROM users.outbox_message_consumers;
		                                            DELETE FROM users.outbox_messages;
		                                            DELETE FROM users.users;
		                                            DELETE FROM users.user_roles;
		                                            """);
	}

	protected async Task<string> GetAccessTokenAsync(string email, string password)
	{
		using var client = new HttpClient();

		var authRequestParameters = new KeyValuePair<string, string>[]
		{
			new("client_id", "_options.PublicClientId"),
			new("scope", "openid"),
			new("grant_type", "password"),
			new("username", email),
			new("password", password)
		};

		using var authRequestContent = new FormUrlEncodedContent(authRequestParameters);

		using var authRequest = new HttpRequestMessage(HttpMethod.Post, new Uri("_options.TokenUrl"));
		authRequest.Content = authRequestContent;

		using var authorizationResponse = await client.SendAsync(authRequest);

		authorizationResponse.EnsureSuccessStatusCode();

		var authToken = await authorizationResponse.Content.ReadFromJsonAsync<AuthToken>();

		return authToken!.AccessToken;
	}

	internal sealed class AuthToken
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; init; }
	}
}