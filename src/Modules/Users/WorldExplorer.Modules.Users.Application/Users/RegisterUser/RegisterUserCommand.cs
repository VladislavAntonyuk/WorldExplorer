using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

using Domain.Users;
using GetUser;
using WorldExplorer.Modules.Users.Application.Abstractions.Identity;

public sealed record RegisterUserCommand(Guid ProviderId) : ICommand<UserResponse>;


public sealed record UserResponse(Guid Id, string Name, string Email, Language Language, UserSettings Settings, string Groups);