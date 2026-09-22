using Microsoft.EntityFrameworkCore;
using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Interfaces.Repository;
using System.Linq.Expressions;

namespace OmnesoftChallenge.DAL.Repository.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected IUnitOfWork _unitOfWork;
    protected EntityFrameworkContext Db;
    protected DbSet<TEntity> DbSet;
    private bool _disposed;

    public Repository(EntityFrameworkContext context, IUnitOfWork unitOfWork)
    {
        Db = context;
        DbSet = Db.Set<TEntity>();
        _unitOfWork = unitOfWork;
    }

    public virtual async Task<TEntity> Insert(TEntity obj)
    {
        _unitOfWork.BeginTransaction();
        var objreturn = await DbSet.AddAsync(obj);
        await _unitOfWork.CommitAsync();
        return objreturn.Entity;
    }

    public async Task Insert(List<TEntity> obj)
    {
        _unitOfWork.BeginTransaction();
        await DbSet.AddRangeAsync(obj);
        await _unitOfWork.CommitAsync();
    }

    public virtual async Task<TEntity?> FindById(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity?> FindById(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        return await DbSet.ToListAsync();
    }

    public virtual TEntity Update(TEntity obj)
    {
        _unitOfWork.BeginTransaction();
        var entry = Db.Entry(obj);
        DbSet.Attach(obj);
        entry.State = EntityState.Modified;
        _unitOfWork.Commit();

        return obj;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity obj)
    {
        _unitOfWork.BeginTransaction();
        DbSet.Update(obj);
        await _unitOfWork.CommitAsync();
        return obj;
    }

    public virtual T Update<T>(T obj) where T : class
    {
        _unitOfWork.BeginTransaction();
        var foreignDbSet = Db.Set<T>();
        var entry = Db.Entry(obj);
        foreignDbSet.Attach(obj);
        entry.State = EntityState.Modified;
        _unitOfWork.Commit();

        return obj;
    }

    public virtual async Task<T> Insert<T>(T obj) where T : class
    {
        _unitOfWork.BeginTransaction();
        var foreignDbSet = Db.Set<T>();
        var objreturn = await foreignDbSet.AddAsync(obj);
        await _unitOfWork.CommitAsync();
        return objreturn.Entity;
    }

    public virtual void UpdateRange(List<TEntity> arrObj)
    {
        _unitOfWork.BeginTransaction();
        DbSet.UpdateRange(arrObj);
        _unitOfWork.Commit();
    }

    public virtual async Task Remove(Guid id)
    {
        var entity = await FindById(id);
        if (entity != null)
        {
            _unitOfWork.BeginTransaction();
            DbSet.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
    }

    public virtual async Task Remove(int id)
    {
        var entity = await FindById(id);
        if (entity != null)
        {
            _unitOfWork.BeginTransaction();
            DbSet.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
    }

    public virtual void Remove(List<TEntity> arrObj)
    {
        _unitOfWork.BeginTransaction();
        DbSet.RemoveRange(arrObj);
        _unitOfWork.Commit();
    }

    public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Where(predicate).ToListAsync();
    }

    public async Task<int> SaveChanges()
    {
        return await Db.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            Db.Dispose();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
