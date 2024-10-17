using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using WorldExplorer.Modules.Users.Presentation.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

using AutoFixture;

public class GetUserProfileTests : BaseIntegrationTest
{
    public GetUserProfileTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_WhenAccessTokenNotProvided()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnOk_WhenUserExists()
    {
        // Arrange
        string accessToken = await RegisterUserAndGetAccessTokenAsync("exists@test.com", Faker.Create<string>());
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        UserResponse? user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
    }

    private async Task<string> RegisterUserAndGetAccessTokenAsync(string email, string password)
    {
        var request = new RegisterUser.Request
        {
            Email = email,
            Password = password,
            FirstName = Faker.Create<string>(),
            LastName = Faker.Create<string>()
        };

        await HttpClient.PostAsJsonAsync("users/register", request);

        string accessToken = await GetAccessTokenAsync(request.Email, request.Password);

        return accessToken;
    }
}
