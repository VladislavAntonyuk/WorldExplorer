namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using Abstractions.Identity;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Users;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository, IGraphClientService graphClientService)
	: ICommandHandler<DeleteUserCommand>
{
	public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetAsync(request.UserId, cancellationToken);

		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound(request.UserId));
		}

		await userRepository.DeleteAsync(request.UserId, cancellationToken);
		await graphClientService.DeleteAsync(user.Id, cancellationToken);

		return Result.Success();
	}
}