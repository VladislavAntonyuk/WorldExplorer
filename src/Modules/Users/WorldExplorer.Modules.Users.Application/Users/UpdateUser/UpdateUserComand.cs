namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using Common.Application.Messaging;

public sealed record UpdateUserCommand(Guid UserId, bool TrackUserLocation) : ICommand;