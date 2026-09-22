using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.Api.Endpoints.Products;

public class GetProductsEndpoint(IProductService productService) : EndpointWithoutRequest<ProductListResponse>
{
    private readonly IProductService _productService = productService;

    public override void Configure()
    {
        Get("/products");
        Policies("AdminOrGuest");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _productService.GetProducts();

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
