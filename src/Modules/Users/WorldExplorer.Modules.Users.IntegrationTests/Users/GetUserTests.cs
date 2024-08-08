using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Modules.Users.Application.Users.RegisterUser;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using FluentAssertions;
using AutoFixture;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class GetUserTests : BaseIntegrationTest
{
    public GetUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        Result<UserResponse> userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Fact]
    public async Task Should_ReturnUser_WhenUserExists()
    {
        // Arrange
        Result<Guid> result = await Sender.Send(new RegisterUserCommand(
            Faker.Create<string>(),
            Faker.Create<string>(),
            Faker.Create<string>(),
            Faker.Create<string>()));
        Guid userId = result.Value;

        // Act
        Result<UserResponse> userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.IsSuccess.Should().BeTrue();
        userResult.Value.Should().NotBeNull();
    }
}
