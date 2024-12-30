namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

using Abstractions;
using Application.Users.GetUser;
using Application.Users.RegisterUser;
using Domain.Users;
using FluentAssertions;

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
