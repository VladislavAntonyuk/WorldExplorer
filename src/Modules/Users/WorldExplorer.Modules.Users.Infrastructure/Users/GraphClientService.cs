namespace WorldExplorer.Modules.Users.Infrastructure.Users;

using System.Globalization;
using Application.Abstractions.Identity;
using Common.Infrastructure;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using AzureGroup = Application.Abstractions.Identity.AzureGroup;
using AzureUser = Application.Abstractions.Identity.AzureUser;

public class GraphClientService : IGraphClientService
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

	public async Task<AzureUser?> GetUser(Guid providerId, CancellationToken cancellationToken)
	{
		User? user;
		try
		{
			user = await graphClient.Users[providerId.ToString()].GetAsync(cancellationToken: cancellationToken);
		}
		catch
		{
			return null;
		}

		if (string.IsNullOrEmpty(user?.Id))
		{
			return null;
		}

		var culture = GetCulture(user.Country);
		return new AzureUser
		{
			Id = user.Id,
			DisplayName = user.DisplayName,
			OtherMails = user.OtherMails ?? Enumerable.Empty<string>(),
			EnableAccessibility = Convert.ToBoolean(GetAdditionalData(user, accessibilityKey)),
			Language = culture.TwoLetterISOLanguageName.GetValueFromDescription<Language>(),
			Groups = await GetUserGroups(providerId, cancellationToken)
		};
	}

	private static object? GetAdditionalData(User user, string key)
	{
		return user.AdditionalData.TryGetValue(key, out var value) ? value : null;
	}

	private async Task<IEnumerable<AzureGroup>> GetUserGroups(Guid? providerId, CancellationToken cancellationToken)
	{
		var membersOf = await graphClient.Users[providerId?.ToString()].MemberOf.GetAsync(cancellationToken: cancellationToken);
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

		return [];
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

	public Task DeleteAsync(Guid providerId, CancellationToken cancellationToken)
	{
		return graphClient.Users[providerId.ToString()].DeleteAsync(cancellationToken: cancellationToken);
	}
}