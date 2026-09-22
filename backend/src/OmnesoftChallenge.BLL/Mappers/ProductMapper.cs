using OmnesoftChallenge.BLL.ViewModels.ProductContracts;
using OmnesoftChallenge.DAL.Entities;
using Riok.Mapperly.Abstractions;

namespace OmnesoftChallenge.BLL.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ProductMapper
{
    public static partial ProductResponse ToProductResponse(this Product product);

    public static partial List<ProductResponse> ToProductResponses(this List<Product> products);

    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.InsertionDate))]
    [MapperIgnoreTarget(nameof(Product.LastUpdateDate))]
    public static partial Product ToProduct(this CreateProductRequest request);

    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.InsertionDate))]
    [MapperIgnoreTarget(nameof(Product.LastUpdateDate))]
    public static partial void ApplyTo(this UpdateProductRequest request, Product product);
}
