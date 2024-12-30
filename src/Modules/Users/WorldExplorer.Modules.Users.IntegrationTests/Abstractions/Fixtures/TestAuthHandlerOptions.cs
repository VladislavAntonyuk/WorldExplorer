namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions.Fixtures;

using Microsoft.AspNetCore.Authentication;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
	public bool FakeSuccessfulAuthentication { get; set; } = true;

	public bool FailPermission { get; set; }
}