using ErrorOr;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.Mappers;
using OmnesoftChallenge.BLL.Utils;
using OmnesoftChallenge.BLL.Utils.Validators;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;
using OmnesoftChallenge.DAL.Entities;
using OmnesoftChallenge.DAL.Interfaces.Repository;

namespace OmnesoftChallenge.BLL.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<ProductListResponse>> GetProducts()
    {
        List<Product> products = await _productRepository.GetAllOrderedById();

        return new ProductListResponse { Products = products.ToProductResponses() };
    }

    public async Task<ErrorOr<ProductResponse>> GetProductById(int id)
    {
        var errors = RequestValidator.ValidateProductId(id);
        if (errors.Count > 0)
        {
            return errors;
        }

        Product? product = await _productRepository.GetById(id);
        if (product is null)
        {
            return ProductNotFound(ErrorCodes.ProductById, id);
        }

        return product.ToProductResponse();
    }

    public async Task<ErrorOr<ProductResponse>> CreateProduct(CreateProductRequest request)
    {
        var errors = RequestValidator.ValidateProduct(request.Name, request.Price, request.Description);
        if (errors.Count > 0)
        {
            return errors;
        }

        var product = request.ToProduct();
        product.InsertionDate = DateTime.UtcNow;
        product.LastUpdateDate = product.InsertionDate;

        product = await _productRepository.Insert(product);

        return product.ToProductResponse();
    }

    public async Task<ErrorOr<ProductResponse>> UpdateProduct(UpdateProductRequest request)
    {
        List<Error> errors =
        [
            .. RequestValidator.ValidateProductId(request.Id),
            .. RequestValidator.ValidateProduct(request.Name, request.Price, request.Description)
        ];
        if (errors.Count > 0)
        {
            return errors;
        }

        Product? product = await _productRepository.GetById(request.Id);
        if (product is null)
        {
            return ProductNotFound(ErrorCodes.UpdateProduct, request.Id);
        }

        request.ApplyTo(product);
        product.LastUpdateDate = DateTime.UtcNow;

        product = await _productRepository.UpdateAsync(product);

        return product.ToProductResponse();
    }

    public async Task<ErrorOr<DeleteProductResponse>> DeleteProduct(int id)
    {
        var errors = RequestValidator.ValidateProductId(id);
        if (errors.Count > 0)
        {
            return errors;
        }

        Product? product = await _productRepository.GetById(id);
        if (product is null)
        {
            return ProductNotFound(ErrorCodes.DeleteProduct, id);
        }

        await _productRepository.Remove(id);

        return new DeleteProductResponse { Id = id, Deleted = true };
    }

    #region Private Helper Methods

    private static Error ProductNotFound(string code, int id)
    {
        return Error.NotFound(
            code: code,
            description: $"Product not found for Id: {id}"
        );
    }

    #endregion
}
