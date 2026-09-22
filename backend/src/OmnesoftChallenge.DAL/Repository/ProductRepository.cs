using Dapper;
using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Entities;
using OmnesoftChallenge.DAL.Interfaces.Repository;
using OmnesoftChallenge.DAL.Repository.Base;
using System.Data;

namespace OmnesoftChallenge.DAL.Repository;

public class ProductRepository(EntityFrameworkContext context, IUnitOfWork unitOfWork, DapperContext dapperContext)
    : Repository<Product>(context, unitOfWork), IProductRepository
{
    private readonly DapperContext _dapperContext = dapperContext;

    public async Task<Product?> GetById(int id)
    {
        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            var query = @"
                SELECT id, name, price, description, insertion_date, last_update_date
                FROM product
                WHERE id = @Id";

            return await db.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
        }
    }

    public async Task<List<Product>> GetAllOrderedById()
    {
        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            var query = @"
                SELECT id, name, price, description, insertion_date, last_update_date
                FROM product
                ORDER BY id";

            var result = await db.QueryAsync<Product>(query);
            return [.. result];
        }
    }
}
