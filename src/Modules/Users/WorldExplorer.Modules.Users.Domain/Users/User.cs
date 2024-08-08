using WorldExplorer.Common.Domain;

namespace WorldExplorer.Modules.Users.Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

	public Guid Id { get; private set; }

	public string IdentityId { get; private set; }

	public UserSettings Settings { get; private set; } = new();

    public static User Create(string identityId, UserSettings userSettings)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            IdentityId = identityId,
            Settings = userSettings
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void Update(UserSettings settings)
    {
        if (Settings == settings)
        {
            return;
        }

        Settings = settings;

        Raise(new UserProfileUpdatedDomainEvent(Id, Settings));
    }
}
