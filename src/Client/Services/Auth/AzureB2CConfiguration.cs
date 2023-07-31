namespace Client.Services.Auth;

internal class AzureB2CConfiguration
{
	public required string ClientId { get; set; }
	public required string IosKeychainSecurityGroups { get; set; }
	public required string AuthoritySignIn { get; set; }
	public required IEnumerable<string> Scopes { get; set; }
	public required string SignInPolicy { get; set; }
}