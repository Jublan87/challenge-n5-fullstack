using N5.Permissions.Domain.Interfaces;
using N5.Permissions.Infrastructure.Persistence;

namespace N5.Permissions.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IPermissionRepository? _permissions;
    private IPermissionTypeRepository? _permissionTypes;
    private bool _disposed = false;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IPermissionRepository Permissions
    {
        get
        {
            _permissions ??= new PermissionRepository(_context);
            return _permissions;
        }
    }

    public IPermissionTypeRepository PermissionTypes
    {
        get
        {
            _permissionTypes ??= new PermissionTypeRepository(_context);
            return _permissionTypes;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

