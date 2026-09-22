using Microsoft.AspNetCore.Mvc.Testing;
using OmnesoftChallenge.IntegrationTests.Contracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OmnesoftChallenge.IntegrationTests.Infrastructure;

public abstract class ApiTestBase(OmnesoftChallengeApiFactory factory)
{
    protected const string AdminUserName = "admin";
    protected const string AdminPassword = "admin123";
    protected const string GuestUserName = "guest";
    protected const string GuestPassword = "guest123";

    protected OmnesoftChallengeApiFactory Factory { get; } = factory;

    protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected HttpClient CreateClient() =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

    protected static async Task<HttpResponseMessage> PostLoginAsync(HttpClient client, string userName, string password) =>
        await client.PostAsJsonAsync("/Auth/login", new { userName, password }, CancellationToken);

    protected async Task<LoginResult> LoginAsync(string userName, string password)
    {
        using var client = CreateClient();
        var response = await PostLoginAsync(client, userName, password);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<LoginResult>(CancellationToken))!;
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(string userName, string password)
    {
        var login = await LoginAsync(userName, password);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        return client;
    }

    protected Task<HttpClient> CreateAdminClientAsync() => CreateAuthenticatedClientAsync(AdminUserName, AdminPassword);

    protected Task<HttpClient> CreateGuestClientAsync() => CreateAuthenticatedClientAsync(GuestUserName, GuestPassword);

    protected static async Task<T> ReadAsync<T>(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<T>(CancellationToken))!;
}
