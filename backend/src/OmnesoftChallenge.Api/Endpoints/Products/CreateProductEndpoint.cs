using FastEndpoints;
using OmnesoftChallenge.Api.Endpoints.Errors;
using OmnesoftChallenge.BLL.Interfaces.Products;
using OmnesoftChallenge.BLL.ViewModels.ProductContracts;

namespace OmnesoftChallenge.Api.Endpoints.Products;

public class CreateProductEndpoint(IProductService productService) : Endpoint<CreateProductRequest, ProductResponse>
{
    private readonly IProductService _productService = productService;

    public override void Configure()
    {
        Post("/products");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var result = await _productService.CreateProduct(req);

        await result.SwitchAsync(
            response => Send.OkAsync(response, ct),
            errors => Send.ResultAsync(errors.ToHandledErrorResult()));
    }
}
