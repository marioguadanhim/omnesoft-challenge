namespace OmnesoftChallenge.BLL.ViewModels.ProductContracts;

public record DeleteProductResponse
{
    public int Id { get; init; }
    public bool Deleted { get; init; }
}
