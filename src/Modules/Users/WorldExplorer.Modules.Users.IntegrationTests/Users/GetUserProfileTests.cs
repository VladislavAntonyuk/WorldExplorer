using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using WorldExplorer.Modules.Users.Presentation.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class GetUserProfileTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
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
        string accessToken = await RegisterUserAndGetAccessTokenAsync();
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

    private async Task<string> RegisterUserAndGetAccessTokenAsync()
    {
        var request = new RegisterUser.Request
        {
	        ClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1",
	        ObjectId = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")
		};

        await HttpClient.PostAsJsonAsync("users/register", request);

        string accessToken = string.Empty;

        return accessToken;
    }
}
