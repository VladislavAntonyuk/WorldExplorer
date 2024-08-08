using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.UnitTests.Abstractions;
using FluentAssertions;

namespace WorldExplorer.Modules.Users.UnitTests.Users;

using AutoFixture;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_ShouldReturnUser()
    {
        // Act
        var user = User.Create(
	        Guid.NewGuid().ToString(),
			Faker.Create<UserSettings>());

        // Assert
        user.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent_WhenUserCreated()
    {
		// Act
		var user = User.Create(
			Guid.NewGuid().ToString(),
			Faker.Create<UserSettings>());

		// Assert
		UserRegisteredDomainEvent domainEvent =
            AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);

        domainEvent.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Update_ShouldRaiseDomainEvent_WhenUserUpdated()
    {
		// Arrange
		var user = User.Create(
			Guid.NewGuid().ToString(),
			Faker.Create<UserSettings>());

		// Act
		user.Update(user.Settings);

        // Assert
        UserProfileUpdatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<UserProfileUpdatedDomainEvent>(user);

        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.Settings.Should().Be(user.Settings);
    }

    [Fact]
    public void Update_ShouldNotRaiseDomainEvent_WhenUserNotUpdated()
    {
		// Arrange
		var user = User.Create(
			Guid.NewGuid().ToString(),
			Faker.Create<UserSettings>());

		user.ClearDomainEvents();

        // Act
        user.Update(user.Settings);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }
}
