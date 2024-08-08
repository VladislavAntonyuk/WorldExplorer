namespace WorldExplorer.Modules.Users.Application.Abstractions.Identity;

public class AzureUser
{
	public string Id { get; set; } = string.Empty;
	public IEnumerable<AzureGroup> Groups { get; set; } = Enumerable.Empty<AzureGroup>();
	public bool EnableAccessibility { get; set; }
	public Language Language { get; set; }
	public string? DisplayName { get; set; }
	public IEnumerable<string> OtherMails { get; set; } = Enumerable.Empty<string>();
}