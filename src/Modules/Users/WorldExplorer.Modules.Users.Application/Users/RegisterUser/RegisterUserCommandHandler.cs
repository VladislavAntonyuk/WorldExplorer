using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Application.Abstractions.Data;
using WorldExplorer.Modules.Users.Domain.Users;

namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        //Result<string> result = await identityProviderService.RegisterUserAsync(
        //    new UserModel(request.Email, request.Password, request.FirstName, request.LastName),
        //    cancellationToken);

        //if (result.IsFailure)
        //{
        //    return Result.Failure<Guid>(result.Error);
        //}

        var user = User.Create(request.Email, request.FirstName, request.LastName, "result.Value");

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
