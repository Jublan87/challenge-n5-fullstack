using Microsoft.EntityFrameworkCore;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Interfaces;
using N5.Permissions.Infrastructure.Persistence;

namespace N5.Permissions.Infrastructure.Repositories;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Permission?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.PermissionType)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.PermissionType)
            .ToListAsync();
    }
}

