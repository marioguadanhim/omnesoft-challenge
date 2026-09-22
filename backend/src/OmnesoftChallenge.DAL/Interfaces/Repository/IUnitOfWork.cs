namespace OmnesoftChallenge.DAL.Interfaces.Repository;

public interface IUnitOfWork
{
    void BeginTransaction();
    void Commit();
    Task CommitAsync();
}
