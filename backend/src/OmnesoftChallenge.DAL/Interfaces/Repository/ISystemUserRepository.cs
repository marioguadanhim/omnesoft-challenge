using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.Interfaces.Repository;

public interface ISystemUserRepository : IRepository<SystemUser>
{
    Task<SystemUser?> GetForLogin(string userName, string hashedPassword);
}
