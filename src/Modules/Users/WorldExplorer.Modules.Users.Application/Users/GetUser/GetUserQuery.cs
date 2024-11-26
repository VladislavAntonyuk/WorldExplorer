namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Common.Application.Messaging;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;