using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.Api.Endpoints.Products;

public class UpdateProductEndpoint(IProductService productService) : Endpoint<UpdateProductRequest, ProductResponse>
{
    private readonly IProductService _productService = productService;

    public override void Configure()
    {
        Put("/products/{id:int}");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        var result = await _productService.UpdateProduct(req);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
