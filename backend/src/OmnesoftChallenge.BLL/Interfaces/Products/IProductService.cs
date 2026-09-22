using ErrorOr;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.BLL.Interfaces.Products;

public interface IProductService
{
    Task<ErrorOr<ProductListResponse>> GetProducts();
    Task<ErrorOr<ProductResponse>> GetProductById(int id);
    Task<ErrorOr<ProductResponse>> CreateProduct(CreateProductRequest request);
    Task<ErrorOr<ProductResponse>> UpdateProduct(UpdateProductRequest request);
    Task<ErrorOr<DeleteProductResponse>> DeleteProduct(int id);
}
