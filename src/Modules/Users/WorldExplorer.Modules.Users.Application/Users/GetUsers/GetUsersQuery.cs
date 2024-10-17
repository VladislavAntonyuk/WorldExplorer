namespace WorldExplorer.Modules.Users.Application.Users.GetUsers;

using Common.Application.Messaging;
using GetUser;

public sealed record GetUsersQuery : IQuery<List<UserResponse>>;