namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Abstractions;
using Application.Users.GetUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Presentation.Users;
using Shouldly;
using Xunit;

public class GetUserProfileTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
	[Fact]
    public async Task Should_ReturnUnauthorized_WhenAccessTokenNotProvided()
    {
		SetAuth(false);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnOk_WhenUserExists()
    {
		SetAuth(true);

		// Arrange
        string accessToken = await RegisterUserAndGetAccessTokenAsync();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("users/profile", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        UserResponse? user = await response.Content.ReadFromJsonAsync<UserResponse>(TestContext.Current.CancellationToken);
        user.ShouldNotBeNull();
    }

    private async Task<string> RegisterUserAndGetAccessTokenAsync()
    {
        var request = new RegisterUser.Request
        {
	        ClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1",
	        ObjectId = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")
		};

        var response = await HttpClient.PostAsJsonAsync("users/register", request);
        response.EnsureSuccessStatusCode();

        string accessToken = string.Empty;

        return accessToken;
    }
}
