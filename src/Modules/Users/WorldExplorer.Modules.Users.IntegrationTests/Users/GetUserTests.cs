namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

using Abstractions;
using Application.Users.GetUser;
using Application.Users.RegisterUser;
using Domain.Users;
using Shouldly;
using Xunit;

public class GetUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var userResult = await Sender.Send(new GetUserQuery(userId), TestContext.Current.CancellationToken);

        // Assert
        userResult.Error.ShouldBe(UserErrors.NotFound(userId));
    }

    [Fact]
    public async Task Should_ReturnUser_WhenUserExists()
    {
        // Arrange
        var result = await Sender.Send(new RegisterUserCommand(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")), TestContext.Current.CancellationToken);
        var userId = result.Value.Id;

        // Act
        var userResult = await Sender.Send(new GetUserQuery(userId), TestContext.Current.CancellationToken);

        // Assert
        userResult.IsSuccess.ShouldBeTrue();
        userResult.Value.ShouldNotBeNull();
    }
}
