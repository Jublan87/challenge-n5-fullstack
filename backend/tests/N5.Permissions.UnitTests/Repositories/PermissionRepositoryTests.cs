using Microsoft.EntityFrameworkCore;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Infrastructure.Persistence;
using N5.Permissions.Infrastructure.Repositories;
using Xunit;

namespace N5.Permissions.UnitTests.Repositories;

public class PermissionRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PermissionRepository _repository;

    public PermissionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new PermissionRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPermissionExists_ReturnsPermissionWithPermissionType()
    {
        // Arrange
        var permissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        var permission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now,
            PermissionType = permissionType
        };

        _context.PermissionTypes.Add(permissionType);
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Juan", result.NombreEmpleado);
        Assert.NotNull(result.PermissionType);
        Assert.Equal("Enfermedad", result.PermissionType.Descripcion);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPermissionDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPermissionsWithPermissionTypes()
    {
        // Arrange
        var permissionType1 = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        var permissionType2 = new PermissionType { Id = 2, Descripcion = "Vacaciones" };

        var permission1 = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now,
            PermissionType = permissionType1
        };

        var permission2 = new Permission
        {
            Id = 2,
            NombreEmpleado = "María",
            ApellidoEmpleado = "López",
            TipoPermiso = 2,
            FechaPermiso = DateTime.Now,
            PermissionType = permissionType2
        };

        _context.PermissionTypes.AddRange(permissionType1, permissionType2);
        _context.Permissions.AddRange(permission1, permission2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.NotNull(result[0].PermissionType);
        Assert.NotNull(result[1].PermissionType);
    }

    [Fact]
    public async Task AddAsync_AddsPermissionToContext()
    {
        // Arrange
        var permission = new Permission
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        // Act
        var result = await _repository.AddAsync(permission);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        var savedPermission = await _context.Permissions.FindAsync(result.Id);
        Assert.NotNull(savedPermission);
        Assert.Equal("Juan", savedPermission.NombreEmpleado);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPermission()
    {
        // Arrange
        var permission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        permission.NombreEmpleado = "Pedro";

        // Act
        var result = await _repository.UpdateAsync(permission);
        await _context.SaveChangesAsync();

        // Assert
        var updatedPermission = await _context.Permissions.FindAsync(1);
        Assert.NotNull(updatedPermission);
        Assert.Equal("Pedro", updatedPermission.NombreEmpleado);
    }

    [Fact]
    public async Task DeleteAsync_WhenPermissionExists_ReturnsTrue()
    {
        // Arrange
        var permissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        var permission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now,
            PermissionType = permissionType
        };

        _context.PermissionTypes.Add(permissionType);
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(1);
        await _context.SaveChangesAsync();

        // Assert
        Assert.True(result);
        var deletedPermission = await _context.Permissions.FindAsync(1);
        Assert.Null(deletedPermission);
    }

    [Fact]
    public async Task DeleteAsync_WhenPermissionDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

