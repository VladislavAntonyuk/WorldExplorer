using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;
