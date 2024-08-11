using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Domain.Users;

namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

using Abstractions.Identity;
using Common.Application.Abstractions.Data;

internal sealed class RegisterUserCommandHandler(
	IUserRepository userRepository,
	IGraphClientService graphClientService,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RegisterUserCommand, UserResponse>
{
	public async Task<Result<UserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
	{
		var profile = await graphClientService.GetUser(request.ProviderId, cancellationToken);
		if (profile is null)
		{
			return Result.Failure<UserResponse>(UserErrors.NotFound(request.ProviderId));
		}

		//Result<string> result = await identityProviderService.RegisterUserAsync(
		//    new UserModel(request.Email, request.Password, request.Name, request.LastName),
		//    cancellationToken);

		//if (result.IsFailure)
		//{
		//    return Result.Failure<Guid>(result.Error);
		//}

		var user = await userRepository.GetAsync(request.ProviderId, cancellationToken);
		if (user is null)
		{
			user = User.Create(request.ProviderId, new UserSettings() { TrackUserLocation = false });
			userRepository.Insert(user);

			await unitOfWork.SaveChangesAsync(cancellationToken);
		}
		
		return new UserResponse(
			user.Id,
			profile.DisplayName ?? string.Empty,
			profile.OtherMails.FirstOrDefault(string.Empty),
			profile.Language,
			user.Settings,
			string.Join(',', profile.Groups.Select(x => x.DisplayName)));
	}
}
