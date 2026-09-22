using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.Interfaces.Repository;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetById(int id);
    Task<List<Product>> GetAllOrderedById();
}
