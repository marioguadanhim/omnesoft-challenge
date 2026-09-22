using OmnesoftChallenge.IntegrationTests.Contracts;
using OmnesoftChallenge.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OmnesoftChallenge.IntegrationTests.Controllers;

[Collection(ApiCollection.Name)]
public class ProductControllerTests(OmnesoftChallengeApiFactory factory) : ApiTestBase(factory)
{
    private const int MissingProductId = 999_999;

    private async Task<ProductResult> CreateProductAsync(HttpClient adminClient, string name, decimal price, string? description = null)
    {
        var response = await adminClient.PostAsJsonAsync("/products", new { name, price, description }, CancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadAsync<ProductResult>(response);
    }

    private static string UniqueName(string prefix) => $"{prefix} {Guid.NewGuid():N}";

    [Fact]
    public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/products", CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithTamperedToken_ReturnsUnauthorized()
    {
        var login = await LoginAsync(AdminUserName, AdminPassword);
        using var client = CreateClient();
        var signatureTail = login.AccessToken[^2..] == "AA" ? "BB" : "AA";
        var tampered = login.AccessToken[..^2] + signatureTail;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tampered);

        var response = await client.GetAsync("/products", CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ReturnsTheSeededProduct()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.GetAsync("/products", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await ReadAsync<ProductListResult>(response);
        var seeded = Assert.Single(list.Products, product => product.Id == 1);
        Assert.Equal("Sample Product", seeded.Name);
        Assert.Equal(10.99m, seeded.Price);
        Assert.Equal("A sample product for testing.", seeded.Description);
    }

    [Fact]
    public async Task GetProductById_WhenProductExists_ReturnsProduct()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.GetAsync("/products/1", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await ReadAsync<ProductResult>(response);
        Assert.Equal(new ProductResult(1, "Sample Product", 10.99m, "A sample product for testing."), product);
    }

    [Fact]
    public async Task GetProductById_WhenProductDoesNotExist_ReturnsNotFound()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.GetAsync($"/products/{MissingProductId}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Equal("ProductByIdError", error.Code);
    }

    [Fact]
    public async Task GetProductById_WithNonPositiveId_ReturnsBadRequest()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.GetAsync("/products/0", CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Contains("Id must be greater than 0.", error.ErrorDescription);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_PersistsProductVisibleToBothReadPaths()
    {
        using var client = await CreateAdminClientAsync();
        var name = UniqueName("Created");

        var created = await CreateProductAsync(client, name, 19.90m, "Created by an integration test");

        Assert.True(created.Id > 1);
        Assert.Equal(name, created.Name);
        Assert.Equal(19.90m, created.Price);

        var byId = await ReadAsync<ProductResult>(await client.GetAsync($"/products/{created.Id}", CancellationToken));
        Assert.Equal(created, byId);

        var list = await ReadAsync<ProductListResult>(await client.GetAsync("/products", CancellationToken));
        Assert.Contains(list.Products, product => product.Id == created.Id && product.Name == name);
    }

    [Fact]
    public async Task CreateProduct_WithoutDescription_PersistsNullDescription()
    {
        using var client = await CreateAdminClientAsync();

        var created = await CreateProductAsync(client, UniqueName("No description"), 5m);

        Assert.Null(created.Description);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidPayload_ReturnsBadRequestWithEveryFailure()
    {
        using var client = await CreateAdminClientAsync();

        var response = await client.PostAsJsonAsync("/products", new { name = string.Empty, price = -1m }, CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Contains("Name cannot be empty.", error.ErrorDescription);
        Assert.Contains("Price must be greater than 0.", error.ErrorDescription);
    }

    [Fact]
    public async Task CreateProduct_WithNameLongerThanTheLimit_ReturnsBadRequest()
    {
        using var client = await CreateAdminClientAsync();

        var response = await client.PostAsJsonAsync("/products", new { name = new string('x', 201), price = 1m }, CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Contains("Name maximum size is 200.", error.ErrorDescription);
    }

    [Fact]
    public async Task CreateProduct_AsGuest_ReturnsForbidden()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.PostAsJsonAsync("/products", new { name = UniqueName("Forbidden"), price = 1m }, CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_AsAdmin_UpdatesEveryField()
    {
        using var client = await CreateAdminClientAsync();
        var created = await CreateProductAsync(client, UniqueName("Before update"), 10m, "before");
        var newName = UniqueName("After update");

        var response = await client.PutAsJsonAsync(
            $"/products/{created.Id}",
            new { name = newName, price = 99.99m, description = "after" },
            CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expected = new ProductResult(created.Id, newName, 99.99m, "after");
        Assert.Equal(expected, await ReadAsync<ProductResult>(response));
        Assert.Equal(expected, await ReadAsync<ProductResult>(await client.GetAsync($"/products/{created.Id}", CancellationToken)));
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        using var client = await CreateAdminClientAsync();

        var response = await client.PutAsJsonAsync(
            $"/products/{MissingProductId}",
            new { name = "Ghost", price = 1m },
            CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Equal("UpdateProductError", error.Code);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidPayload_ReturnsBadRequest()
    {
        using var client = await CreateAdminClientAsync();
        var created = await CreateProductAsync(client, UniqueName("Valid"), 10m);

        var response = await client.PutAsJsonAsync(
            $"/products/{created.Id}",
            new { name = string.Empty, price = 0m },
            CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_AsGuest_ReturnsForbidden()
    {
        using var client = await CreateGuestClientAsync();

        var response = await client.PutAsJsonAsync("/products/1", new { name = "Hijacked", price = 1m }, CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AsAdmin_RemovesTheProduct()
    {
        using var client = await CreateAdminClientAsync();
        var created = await CreateProductAsync(client, UniqueName("To delete"), 3m);

        var response = await client.DeleteAsync($"/products/{created.Id}", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new DeleteProductResult(created.Id, true), await ReadAsync<DeleteProductResult>(response));

        var afterDelete = await client.GetAsync($"/products/{created.Id}", CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        using var client = await CreateAdminClientAsync();

        var response = await client.DeleteAsync($"/products/{MissingProductId}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await ReadAsync<ErrorResult>(response);
        Assert.Equal("DeleteProductError", error.Code);
    }

    [Fact]
    public async Task DeleteProduct_AsGuest_ReturnsForbiddenAndKeepsTheProduct()
    {
        using var adminClient = await CreateAdminClientAsync();
        var created = await CreateProductAsync(adminClient, UniqueName("Protected"), 7m);
        using var guestClient = await CreateGuestClientAsync();

        var response = await guestClient.DeleteAsync($"/products/{created.Id}", CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var stillThere = await adminClient.GetAsync($"/products/{created.Id}", CancellationToken);
        Assert.Equal(HttpStatusCode.OK, stillThere.StatusCode);
    }
}
