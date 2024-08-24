namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Application.Abstractions.Identity;
using WorldExplorer.Modules.Users.Domain.Users;

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
