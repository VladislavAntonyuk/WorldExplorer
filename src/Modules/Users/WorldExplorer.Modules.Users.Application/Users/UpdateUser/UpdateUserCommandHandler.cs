using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Domain.Users;

namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using Abstractions.Identity;
using Common.Application.Abstractions.Data;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository,  IUnitOfWork unitOfWork)
	: ICommandHandler<UpdateUserCommand>
{
	public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		User? user = await userRepository.GetAsync(request.UserId, cancellationToken);

		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound(request.UserId));
		}

		user.Update(new UserSettings() { TrackUserLocation = request.TrackUserLocation });
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
