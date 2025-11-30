using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Interfaces;
using N5.Permissions.Infrastructure.Persistence;

namespace N5.Permissions.Infrastructure.Repositories;

public class PermissionTypeRepository : GenericRepository<PermissionType>, IPermissionTypeRepository
{
    public PermissionTypeRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}

