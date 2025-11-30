using N5.Permissions.Domain.Entities;

namespace N5.Permissions.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission> AddAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(int id);
}

