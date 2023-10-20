namespace WebApp.Services.User;

using Microsoft.Graph.Beta;
using System.Globalization;
using Microsoft.Graph.Beta.Models;
using Shared.Enums;
using Shared.Extensions;

public interface IGraphClientService
{
	Task<AzureUser> GetUser(string providerId, CancellationToken cancellationToken);
}
public class AzureUser
{
	public string Id { get; set; } = string.Empty;
	public IEnumerable<AzureGroup> Groups { get; set; } = new List<AzureGroup>();
	public bool EnableAccessibility { get; set; }
	public Language Language { get; set; }
}

public class AzureGroup
{
	public string? Id { get; set; }
	public string? DisplayName { get; set; }
}

class GraphClientService : IGraphClientService
{
	private readonly string accessibilityKey;
	private readonly CultureInfo defaultCultureInfo;
	private readonly GraphServiceClient graphClient;

	public GraphClientService(GraphServiceClient graphClient, string? defaultAppId)
	{
		ArgumentException.ThrowIfNullOrEmpty(defaultAppId);
		this.graphClient = graphClient;
		accessibilityKey = $"extension_{defaultAppId}_Accessibility";
		defaultCultureInfo = new CultureInfo("en-US");
	}

	public async Task<AzureUser> GetUser(string providerId, CancellationToken cancellationToken)
	{
		var user = await graphClient.Users[providerId].GetAsync(cancellationToken: cancellationToken);
		ArgumentException.ThrowIfNullOrEmpty(user?.Id);

		var culture = GetCulture(user.Country);
		return new AzureUser
		{
			Id = user.Id,
			EnableAccessibility = Convert.ToBoolean(GetAdditionalData(user, accessibilityKey)),
			Language = culture.TwoLetterISOLanguageName.GetValueFromDescription<Language>(),
			Groups = await GetUserGroups(providerId, cancellationToken)
		};
	}

	private static object? GetAdditionalData(User user, string key)
	{
		return user.AdditionalData.TryGetValue(key, out var value) ? value : null;
	}

	private async Task<IEnumerable<AzureGroup>> GetUserGroups(string? providerId, CancellationToken cancellationToken)
	{
		var membersOf = await graphClient.Users[providerId].MemberOf.GetAsync(cancellationToken: cancellationToken);
		if (membersOf?.Value?.Count > 0)
		{
			var groups = membersOf.Value.Select(directoryObject => directoryObject as Group)
								  .Where(group => group is not null)
								  .ToArray();
			if (groups.Length > 0)
			{
				return groups.Select(group => new AzureGroup
				{
					Id = group?.Id,
					DisplayName = group?.DisplayName
				});
			}

			return membersOf.Value.Select(group => new AzureGroup
			{
				Id = group.Id,
				DisplayName = group.AdditionalData.FirstOrDefault(x => x.Key == "displayName").Value?.ToString()
			});
		}

		return Enumerable.Empty<AzureGroup>();
	}

	private CultureInfo GetCulture(string? countryName)
	{
		if (string.IsNullOrWhiteSpace(countryName))
		{
			return defaultCultureInfo;
		}

		return CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
						  .LastOrDefault(x => x.EnglishName.Contains(countryName), defaultCultureInfo);
	}
}

