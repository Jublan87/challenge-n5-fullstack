using Microsoft.EntityFrameworkCore;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Infrastructure.Persistence;
using N5.Permissions.Infrastructure.Repositories;
using Xunit;

namespace N5.Permissions.UnitTests.Repositories;

public class PermissionTypeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PermissionTypeRepository _repository;

    public PermissionTypeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new PermissionTypeRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPermissionTypeExists_ReturnsPermissionType()
    {
        // Arrange
        var permissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        _context.PermissionTypes.Add(permissionType);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Enfermedad", result.Descripcion);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPermissionTypeDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPermissionTypes()
    {
        // Arrange
        var permissionType1 = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        var permissionType2 = new PermissionType { Id = 2, Descripcion = "Vacaciones" };

        _context.PermissionTypes.AddRange(permissionType1, permissionType2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, pt => pt.Descripcion == "Enfermedad");
        Assert.Contains(result, pt => pt.Descripcion == "Vacaciones");
    }

    [Fact]
    public async Task ExistsAsync_WhenPermissionTypeExists_ReturnsTrue()
    {
        // Arrange
        var permissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        _context.PermissionTypes.Add(permissionType);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenPermissionTypeDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

