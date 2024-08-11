using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
