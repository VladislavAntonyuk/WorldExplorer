using System.Net;
using System.Net.Http.Json;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using WorldExplorer.Modules.Users.Presentation.Users;
using FluentAssertions;
using AutoFixture;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class RegisterUserTests : BaseIntegrationTest
{
    public RegisterUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    public static readonly TheoryData<string, string, string, string> InvalidRequests = new()
    {
        { "", Faker.Create<string>(), Faker.Create<string>(), Faker.Create<string>() },
        { Faker.Create<string>(), "", Faker.Create<string>(), Faker.Create<string>() },
        { Faker.Create<string>(), "12345", Faker.Create<string>(), Faker.Create<string>() },
        { Faker.Create<string>(), Faker.Create<string>(), "", Faker.Create<string>() },
        { Faker.Create<string>(), Faker.Create<string>(), Faker.Create<string>(), "" }
    };


    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task Should_ReturnBadRequest_WhenRequestIsNotValid(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = "create@test.com",
            Password = Faker.Create<string>(),
            FirstName = Faker.Create<string>(),
            LastName = Faker.Create<string>()
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnAccessToken_WhenUserIsRegistered()
    {
        // Arrange
        var request = new RegisterUser.Request
        {
            Email = "token@test.com",
            Password = Faker.Create<string>(),
            FirstName = Faker.Create<string>(),
            LastName = Faker.Create<string>()
        };

        await HttpClient.PostAsJsonAsync("users/register", request);

        // Act
        string accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        // Assert
        accessToken.Should().NotBeEmpty();
    }
}
