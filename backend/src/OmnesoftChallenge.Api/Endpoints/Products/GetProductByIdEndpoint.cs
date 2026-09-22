using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.Api.Endpoints.Products;

public class GetProductByIdEndpoint(IProductService productService) : Endpoint<ProductIdRequest, ProductResponse>
{
    private readonly IProductService _productService = productService;

    public override void Configure()
    {
        Get("/products/{id:int}");
        Policies("AdminOrGuest");
    }

    public override async Task HandleAsync(ProductIdRequest req, CancellationToken ct)
    {
        var result = await _productService.GetProductById(req.Id);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
