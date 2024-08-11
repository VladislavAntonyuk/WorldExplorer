using System.Data.Common;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Domain.Users;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Abstractions.Identity;

internal sealed class GetUserQueryHandler(IUserRepository userRepository, IGraphClientService graphClientService)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(request.UserId));
        }

        var profile = await graphClientService.GetUser(user.Id, cancellationToken);
        if (profile is null)
        {
	        return Result.Failure<UserResponse>(UserErrors.NotFound(user.Id));
        }

        return new UserResponse(
	        user.Id,
	        profile.DisplayName ?? string.Empty,
	        profile.OtherMails.FirstOrDefault(string.Empty),
	        profile.Language,
	        user.Settings);
    }
}
