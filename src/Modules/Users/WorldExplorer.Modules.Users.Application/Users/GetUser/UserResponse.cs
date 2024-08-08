namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Abstractions.Identity;
using Domain.Users;

public sealed record UserResponse(Guid Id, string Name, string Email, Language Language, UserSettings Settings);
