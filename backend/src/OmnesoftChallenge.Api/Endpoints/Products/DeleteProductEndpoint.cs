using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.Api.Endpoints.Products;

public class DeleteProductEndpoint(IProductService productService) : Endpoint<ProductIdRequest, DeleteProductResponse>
{
    private readonly IProductService _productService = productService;

    public override void Configure()
    {
        Delete("/products/{id:int}");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(ProductIdRequest req, CancellationToken ct)
    {
        var result = await _productService.DeleteProduct(req.Id);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
