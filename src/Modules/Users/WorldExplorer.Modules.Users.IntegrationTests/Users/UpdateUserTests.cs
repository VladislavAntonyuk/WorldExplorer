using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Users.Application.Users.RegisterUser;
using WorldExplorer.Modules.Users.Application.Users.UpdateUser;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using FluentAssertions;
using AutoFixture;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class UpdateUserTests : BaseIntegrationTest
{
    public UpdateUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    public static readonly TheoryData<UpdateUserCommand> InvalidCommands = new()
    {
        new UpdateUserCommand(Guid.Empty, Faker.Create<string>(), Faker.Create<string>()),
        new UpdateUserCommand(Guid.NewGuid(), "", Faker.Create<string>()),
        new UpdateUserCommand(Guid.NewGuid(), Faker.Create<string>(), "")
    };

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
            new UpdateUserCommand(userId, Faker.Create<string>(), Faker.Create<string>()));

        // Assert
        updateResult.Error.Should().Be(UserErrors.NotFound(userId));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenUserExists()
    {
        // Arrange
        Result<Guid> result = await Sender.Send(new RegisterUserCommand(
            Faker.Create<string>(),
            Faker.Create<string>(),
            Faker.Create<string>(),
            Faker.Create<string>()));

        Guid userId = result.Value;

        // Act
        Result updateResult = await Sender.Send(
            new UpdateUserCommand(userId, Faker.Create<string>(), Faker.Create<string>()));

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
    }
}
