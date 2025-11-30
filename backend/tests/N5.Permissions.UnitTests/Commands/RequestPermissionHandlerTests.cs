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

public class RequestPermissionHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<IElasticsearchService> _elasticsearchMock;
    private readonly Mock<IKafkaProducerService> _kafkaMock;
    private readonly RequestPermissionHandler _handler;

    public RequestPermissionHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _elasticsearchMock = new Mock<IElasticsearchService>();
        _kafkaMock = new Mock<IKafkaProducerService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new RequestPermissionHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _elasticsearchMock.Object,
            _kafkaMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPermissionTypeExists_ReturnsPermissionDto()
    {
        // Arrange
        var dto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var permissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" };
        var permission = new Permission
        {
            Id = 1,
            NombreEmpleado = dto.NombreEmpleado,
            ApellidoEmpleado = dto.ApellidoEmpleado,
            TipoPermiso = dto.TipoPermiso,
            FechaPermiso = dto.FechaPermiso,
            PermissionType = permissionType
        };

        var permissionTypeRepoMock = new Mock<IPermissionTypeRepository>();
        permissionTypeRepoMock.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(x => x.PermissionTypes).Returns(permissionTypeRepoMock.Object);

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.AddAsync(It.IsAny<Permission>())).ReturnsAsync(permission);
        permissionRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(permission);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.NombreEmpleado, result.NombreEmpleado);
        Assert.Equal(dto.ApellidoEmpleado, result.ApellidoEmpleado);
        Assert.Equal(dto.TipoPermiso, result.TipoPermiso);
        _elasticsearchMock.Verify(x => x.IndexPermissionAsync(It.IsAny<Permission>()), Times.Once);
        _kafkaMock.Verify(x => x.PublishOperationAsync(OperationType.Request), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPermissionTypeDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 999,
            FechaPermiso = DateTime.Now
        };

        var permissionTypeRepoMock = new Mock<IPermissionTypeRepository>();
        permissionTypeRepoMock.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.PermissionTypes).Returns(permissionTypeRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(dto));
        _elasticsearchMock.Verify(x => x.IndexPermissionAsync(It.IsAny<Permission>()), Times.Never);
        _kafkaMock.Verify(x => x.PublishOperationAsync(It.IsAny<OperationType>()), Times.Never);
    }
}

