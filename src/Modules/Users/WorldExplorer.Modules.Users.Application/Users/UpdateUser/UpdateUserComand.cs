using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using Domain.Users;

public sealed record UpdateUserCommand(Guid UserId, bool TrackUserLocation) : ICommand;
