using WorldExplorer.Common.Domain;

namespace WorldExplorer.Modules.Users.UnitTests.Abstractions;

using AutoFixture;

public abstract class BaseTest
{
    protected static readonly Fixture Faker = new();

    public static T AssertDomainEventWasPublished<T>(Entity entity)
        where T : IDomainEvent
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        if (domainEvent is null)
        {
            throw new Exception($"{typeof(T).Name} was not published");
        }

        return domainEvent;
    }
}
