namespace OmnesoftChallenge.BLL.ViewModels.ProductContracts;

public record ProductListResponse
{
    public List<ProductResponse> Products { get; init; } = [];
}
