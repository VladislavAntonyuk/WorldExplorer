namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Users;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<UpdateUserCommand>
{
	public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetAsync(request.UserId, cancellationToken);

		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound(request.UserId));
		}

		user.Update(new UserSettings
		{
			TrackUserLocation = request.TrackUserLocation
		});
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}