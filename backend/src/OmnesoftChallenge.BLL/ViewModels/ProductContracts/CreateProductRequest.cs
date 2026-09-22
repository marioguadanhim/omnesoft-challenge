namespace OmnesoftChallenge.BLL.ViewModels.ProductContracts;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}
