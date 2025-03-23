namespace WorldExplorer.Modules.Users.UnitTests.Users;

using Abstractions;
using AutoFixture;
using Domain.Users;
using Shouldly;
using Xunit;

public class UserTests : BaseTest
{
	[Fact]
	public void Create_ShouldReturnUser()
	{
		// Act
		var user = User.Create(Guid.NewGuid(), Faker.Create<UserSettings>());

		// Assert
		user.ShouldNotBeNull();
	}

	[Fact]
	public void Create_ShouldRaiseDomainEvent_WhenUserCreated()
	{
		// Act
		var user = User.Create(Guid.NewGuid(), Faker.Create<UserSettings>());

		// Assert
		var domainEvent = AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);

		domainEvent.UserId.ShouldBe(user.Id);
	}

	[Fact]
	public void Update_ShouldRaiseDomainEvent_WhenUserUpdated()
	{
		// Arrange
		var user = User.Create(Guid.NewGuid(), Faker.Create<UserSettings>());

		// Act
		user.Update(new UserSettings
		{
			TrackUserLocation = true
		});

		// Assert
		var domainEvent = AssertDomainEventWasPublished<UserProfileUpdatedDomainEvent>(user);

		domainEvent.UserId.ShouldBe(user.Id);
		domainEvent.Settings.ShouldBe(user.Settings);
	}

	[Fact]
	public void Update_ShouldNotRaiseDomainEvent_WhenUserNotUpdated()
	{
		// Arrange
		var user = User.Create(Guid.NewGuid(), Faker.Create<UserSettings>());

		user.ClearDomainEvents();

		// Act
		user.Update(user.Settings);

		// Assert
		user.DomainEvents.ShouldBeEmpty();
	}
}