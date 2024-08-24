namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using WorldExplorer.Common.Application.Messaging;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
