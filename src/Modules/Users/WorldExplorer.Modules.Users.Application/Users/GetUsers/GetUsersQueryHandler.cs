namespace WorldExplorer.Modules.Users.Application.Users.GetUsers;

using Abstractions.Identity;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Users;
using GetUser;

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

			users.Add(new UserResponse(user.Id, profile.DisplayName ?? string.Empty,
			                           profile.OtherMails.FirstOrDefault(string.Empty), profile.Language,
			                           user.Settings));
		}

		return users;
	}
}