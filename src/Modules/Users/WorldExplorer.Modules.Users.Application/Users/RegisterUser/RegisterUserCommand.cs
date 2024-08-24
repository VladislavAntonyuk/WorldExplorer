namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

using Abstractions.Identity;
using Common.Application.Messaging;
using Domain.Users;

public sealed record RegisterUserCommand(Guid ProviderId) : ICommand<UserResponse>;


public sealed record UserResponse(Guid Id, string Name, string Email, Language Language, UserSettings Settings, string Groups);