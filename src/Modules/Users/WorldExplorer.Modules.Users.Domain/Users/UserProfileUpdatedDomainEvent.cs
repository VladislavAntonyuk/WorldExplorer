﻿using WorldExplorer.Common.Domain;

namespace WorldExplorer.Modules.Users.Domain.Users;

public sealed class UserProfileUpdatedDomainEvent(Guid userId, UserSettings settings) : DomainEvent
{
    public Guid UserId { get; init; } = userId;

    public UserSettings Settings { get; init; } = settings;
}