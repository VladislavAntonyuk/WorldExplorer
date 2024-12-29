using System.Net;
using System.Net.Http.Json;
using WorldExplorer.Modules.Users.IntegrationTests.Abstractions;
using WorldExplorer.Modules.Users.Presentation.Users;
using FluentAssertions;

namespace WorldExplorer.Modules.Users.IntegrationTests.Users;

public class RegisterUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
	public static readonly TheoryData<string, string> InvalidRequests = new()
    {
	    {"3c3bdb4b-327b-49a9-a13e-0b565526b8a1", ""},
	    {"", "19d3b2c7-8714-4851-ac73-95aeecfba3a6"}
	};


    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task Should_ReturnBadRequest_WhenRequestIsNotValid(
        string clientId,
        string objectId)
    {
        // Arrange
        var request = new RegisterUser.Request
        {
	        ClientId = clientId,
	        ObjectId = Guid.Parse(objectId)
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
	        ClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1",
	        ObjectId = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")
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
			ClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1",
			ObjectId = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6")
		};

        await HttpClient.PostAsJsonAsync("users/register", request);

        // Act
        string accessToken = await GetAccessTokenAsync();

        // Assert
        accessToken.Should().NotBeEmpty();
    }
}
