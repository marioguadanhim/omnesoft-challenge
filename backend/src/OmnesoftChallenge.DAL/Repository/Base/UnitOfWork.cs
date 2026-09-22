using OmnesoftChallenge.DAL.Context;
using OmnesoftChallenge.DAL.Interfaces.Repository;

namespace OmnesoftChallenge.DAL.Repository.Base;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly EntityFrameworkContext _context;
    private bool _disposed;

    public UnitOfWork(EntityFrameworkContext context)
    {
        _context = context;
    }

    public void BeginTransaction()
    {
        _disposed = false;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
