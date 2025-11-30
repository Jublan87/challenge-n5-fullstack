using AutoMapper;
using Moq;
using N5.Permissions.Application.Commands;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Mappings;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;
using Xunit;

namespace N5.Permissions.UnitTests.Commands;

public class ModifyPermissionHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<IElasticsearchService> _elasticsearchMock;
    private readonly Mock<IKafkaProducerService> _kafkaMock;
    private readonly ModifyPermissionHandler _handler;

    public ModifyPermissionHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _elasticsearchMock = new Mock<IElasticsearchService>();
        _kafkaMock = new Mock<IKafkaProducerService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new ModifyPermissionHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _elasticsearchMock.Object,
            _kafkaMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPermissionExists_ReturnsUpdatedPermissionDto()
    {
        // Arrange
        var existingPermission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now,
            PermissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" }
        };

        var dto = new UpdatePermissionDto
        {
            NombreEmpleado = "Pedro",
            ApellidoEmpleado = "López"
        };

        var updatedPermission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Pedro",
            ApellidoEmpleado = "López",
            TipoPermiso = 1,
            FechaPermiso = existingPermission.FechaPermiso,
            PermissionType = existingPermission.PermissionType
        };

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingPermission);
        permissionRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Permission>())).ReturnsAsync(updatedPermission);
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(updatedPermission);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var permissionTypeRepoMock = new Mock<IPermissionTypeRepository>();
        _unitOfWorkMock.Setup(x => x.PermissionTypes).Returns(permissionTypeRepoMock.Object);

        // Act
        var result = await _handler.Handle((1, dto));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pedro", result.NombreEmpleado);
        Assert.Equal("López", result.ApellidoEmpleado);
        _elasticsearchMock.Verify(x => x.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);
        _kafkaMock.Verify(x => x.PublishOperationAsync(OperationType.Modify), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPermissionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var dto = new UpdatePermissionDto
        {
            NombreEmpleado = "Pedro"
        };

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Permission?)null);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);

        // Act
        var result = await _handler.Handle((999, dto));

        // Assert
        Assert.Null(result);
        _elasticsearchMock.Verify(x => x.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Never);
        _kafkaMock.Verify(x => x.PublishOperationAsync(It.IsAny<OperationType>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdatingTipoPermiso_ValidatesPermissionTypeExists()
    {
        // Arrange
        var existingPermission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var dto = new UpdatePermissionDto
        {
            TipoPermiso = 2
        };

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingPermission);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);

        var permissionTypeRepoMock = new Mock<IPermissionTypeRepository>();
        permissionTypeRepoMock.Setup(x => x.ExistsAsync(2)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.PermissionTypes).Returns(permissionTypeRepoMock.Object);

        var updatedPermission = new Permission
        {
            Id = 1,
            NombreEmpleado = existingPermission.NombreEmpleado,
            ApellidoEmpleado = existingPermission.ApellidoEmpleado,
            TipoPermiso = 2,
            FechaPermiso = existingPermission.FechaPermiso
        };

        permissionRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Permission>())).ReturnsAsync(updatedPermission);
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(updatedPermission);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle((1, dto));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TipoPermiso);
    }

    [Fact]
    public async Task Handle_WhenUpdatingTipoPermisoThatDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var existingPermission = new Permission
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var dto = new UpdatePermissionDto
        {
            TipoPermiso = 999
        };

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingPermission);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);

        var permissionTypeRepoMock = new Mock<IPermissionTypeRepository>();
        permissionTypeRepoMock.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.PermissionTypes).Returns(permissionTypeRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle((1, dto)));
    }
}

