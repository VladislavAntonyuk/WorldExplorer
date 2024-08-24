namespace WorldExplorer.Modules.Users.Application.Users.GetUsers;

using GetUser;
using WorldExplorer.Common.Application.Messaging;

public sealed record GetUsersQuery : IQuery<List<UserResponse>>;
