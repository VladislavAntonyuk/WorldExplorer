using System.Data.Common;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Domain.Users;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Abstractions.Identity;

internal sealed class GetUsersQueryHandler(IUserRepository userRepository, IGraphClientService graphClientService)
    : IQueryHandler<GetUsersQuery, List<UserResponse>>
{
    public async Task<Result<List<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var dbUsers = await userRepository.GetAsync(cancellationToken);

		var users = new List<UserResponse>();
		foreach (var user in dbUsers)
		{
			var profile = await graphClientService.GetUser(user.Id, cancellationToken);
			if (profile is null)
			{
				continue;
			}

			users.Add(new UserResponse(user.Id,
			                           profile.DisplayName ?? string.Empty,
			                           profile.OtherMails.FirstOrDefault(string.Empty),
			                           profile.Language,
			                           user.Settings));
		}

		return users;
	}
}
