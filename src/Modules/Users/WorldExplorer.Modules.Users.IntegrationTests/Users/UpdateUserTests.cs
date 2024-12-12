using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Application.Users.RegisterUser;
using WorldExplorer.Modules.Users.Application.Users.UpdateUser;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using FluentAssertions;
using AutoFixture;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class UpdateUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
	public static readonly TheoryData<UpdateUserCommand> InvalidCommands =
	[
		new UpdateUserCommand(Guid.Empty, Faker.Create<bool>()),
		new UpdateUserCommand(Guid.NewGuid(), Faker.Create<bool>())
	];

    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public async Task Should_ReturnError_WhenCommandIsNotValid(UpdateUserCommand command)
    {
        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Should_ReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        Result updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Create<bool>()));

        // Assert
        updateResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenUserExists()
    {
        // Arrange
        var result = await Sender.Send(new RegisterUserCommand(Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")));

        Guid userId = result.Value.Id;

        // Act
        Result updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Create<bool>()));

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
    }
}
