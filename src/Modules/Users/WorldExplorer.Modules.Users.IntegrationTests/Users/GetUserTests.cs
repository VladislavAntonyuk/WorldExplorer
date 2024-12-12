using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Modules.Users.Application.Users.RegisterUser;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using FluentAssertions;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class GetUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Fact]
    public async Task Should_ReturnUser_WhenUserExists()
    {
        // Arrange
        var result = await Sender.Send(new RegisterUserCommand(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")));
        var userId = result.Value.Id;

        // Act
        var userResult = await Sender.Send(new GetUserQuery(userId));

        // Assert
        userResult.IsSuccess.Should().BeTrue();
        userResult.Value.Should().NotBeNull();
    }
}
