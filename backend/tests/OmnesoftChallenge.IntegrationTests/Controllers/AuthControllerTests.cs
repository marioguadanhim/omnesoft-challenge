using OmnesoftChallenge.IntegrationTests.Contracts;
using OmnesoftChallenge.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OmnesoftChallenge.IntegrationTests.Controllers;

[Collection(ApiCollection.Name)]
public class AuthControllerTests(OmnesoftChallengeApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task Login_WithValidAdminCredentials_ReturnsTokenPair()
    {
        using var client = CreateClient();

        var response = await PostLoginAsync(client, AdminUserName, AdminPassword);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var login = await ReadAsync<LoginResult>(response);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));
        Assert.Equal(AdminUserName, login.UserName);
        Assert.Equal("Admin", login.Role);
        Assert.True(login.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithValidGuestCredentials_ReturnsGuestRole()
    {
        var login = await LoginAsync(GuestUserName, GuestPassword);

        Assert.Equal(GuestUserName, login.UserName);
        Assert.Equal("Guest", login.Role);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await PostLoginAsync(client, AdminUserName, "wrong-password");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Equal("AuthenticationError", error.Code);
        Assert.Contains("Invalid username or password", error.ErrorDescription);
    }

    [Fact]
    public async Task Login_WithUnknownUser_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await PostLoginAsync(client, "nobody", "whatever");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
    {
        using var client = CreateClient();

        var response = await PostLoginAsync(client, string.Empty, string.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Contains("UserName cannot be empty.", error.ErrorDescription);
        Assert.Contains("Password cannot be empty.", error.ErrorDescription);
    }

    [Fact]
    public async Task Refresh_WithValidToken_IssuesNewPairAndRevokesThePresentedToken()
    {
        var login = await LoginAsync(AdminUserName, AdminPassword);
        using var client = CreateClient();

        var refreshResponse = await client.PostAsJsonAsync("/Auth/refresh", new { refreshToken = login.RefreshToken }, CancellationToken);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshed = await ReadAsync<LoginResult>(refreshResponse);
        Assert.NotEqual(login.RefreshToken, refreshed.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(refreshed.AccessToken));

        var reuseResponse = await client.PostAsJsonAsync("/Auth/refresh", new { refreshToken = login.RefreshToken }, CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);
        var error = await ReadAsync<ErrorResult>(reuseResponse);
        Assert.Equal("RefreshTokenError", error.Code);
    }

    [Fact]
    public async Task Refresh_IssuesAnAccessTokenThatTheApiAccepts()
    {
        var login = await LoginAsync(AdminUserName, AdminPassword);
        using var client = CreateClient();

        var refreshResponse = await client.PostAsJsonAsync("/Auth/refresh", new { refreshToken = login.RefreshToken }, CancellationToken);
        var refreshed = await ReadAsync<LoginResult>(refreshResponse);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
        var productsResponse = await client.GetAsync("/products", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, productsResponse.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithUnknownToken_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync("/Auth/refresh", new { refreshToken = "not-a-real-token" }, CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithEmptyToken_ReturnsBadRequest()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync("/Auth/refresh", new { refreshToken = string.Empty }, CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
