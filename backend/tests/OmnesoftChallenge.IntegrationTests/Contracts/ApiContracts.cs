namespace OmnesoftChallenge.IntegrationTests.Contracts;

public record LoginResult(string AccessToken, string RefreshToken, DateTime ExpiresAt, string UserName, string Role);

public record ProductResult(int Id, string Name, decimal Price, string? Description);

public record ProductListResult(List<ProductResult> Products);

public record DeleteProductResult(int Id, bool Deleted);

public record ErrorResult(string Code, string ErrorDescription);
