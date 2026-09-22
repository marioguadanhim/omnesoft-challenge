using System.Linq.Expressions;

namespace OmnesoftChallenge.DAL.Interfaces.Repository;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    Task<TEntity> Insert(TEntity obj);
    Task Insert(List<TEntity> obj);
    Task<TEntity?> FindById(Guid id);
    Task<TEntity?> FindById(int id);
    Task<IEnumerable<TEntity>> GetAll();
    TEntity Update(TEntity obj);
    Task<TEntity> UpdateAsync(TEntity obj);
    Task Remove(Guid id);
    Task Remove(int id);
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    Task<int> SaveChanges();
    void Remove(List<TEntity> arrObj);
    void UpdateRange(List<TEntity> arrObj);

    Task<T> Insert<T>(T obj) where T : class;
    T Update<T>(T obj) where T : class;
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
}
