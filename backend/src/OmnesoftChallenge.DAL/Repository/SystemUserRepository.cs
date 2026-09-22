using Dapper;
using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Entities;
using OmnesoftChallenge.DAL.Interfaces.Repository;
using OmnesoftChallenge.DAL.Repository.Base;
using System.Data;

namespace OmnesoftChallenge.DAL.Repository;

public class SystemUserRepository(EntityFrameworkContext context, IUnitOfWork unitOfWork, DapperContext dapperContext)
    : Repository<SystemUser>(context, unitOfWork), ISystemUserRepository
{
    private readonly DapperContext _dapperContext = dapperContext;

    public async Task<SystemUser?> GetForLogin(string userName, string hashedPassword)
    {
        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            var query = @"
                SELECT id, user_name, password, role, active
                FROM ""system_user""
                WHERE user_name = @UserName AND password = @Password
                LIMIT 1";

            return await db.QueryFirstOrDefaultAsync<SystemUser>(query, new { UserName = userName, Password = hashedPassword });
        }
    }
}
