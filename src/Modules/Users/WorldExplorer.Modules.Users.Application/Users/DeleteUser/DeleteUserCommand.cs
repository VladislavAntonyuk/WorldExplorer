namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using Common.Application.Messaging;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;