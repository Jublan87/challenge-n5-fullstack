using AutoMapper;
using Moq;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Mappings;
using N5.Permissions.Application.Queries;
using N5.Permissions.Domain.Entities;
using N5.Permissions.Domain.Enums;
using N5.Permissions.Domain.Interfaces;
using Xunit;

namespace N5.Permissions.UnitTests.Queries;

public class GetPermissionsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<IKafkaProducerService> _kafkaMock;
    private readonly GetPermissionsHandler _handler;

    public GetPermissionsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _kafkaMock = new Mock<IKafkaProducerService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetPermissionsHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _kafkaMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsListOfPermissionDtos()
    {
        // Arrange
        var permissions = new List<Permission>
        {
            new Permission
            {
                Id = 1,
                NombreEmpleado = "Juan",
                ApellidoEmpleado = "García",
                TipoPermiso = 1,
                FechaPermiso = DateTime.Now,
                PermissionType = new PermissionType { Id = 1, Descripcion = "Enfermedad" }
            },
            new Permission
            {
                Id = 2,
                NombreEmpleado = "María",
                ApellidoEmpleado = "López",
                TipoPermiso = 2,
                FechaPermiso = DateTime.Now,
                PermissionType = new PermissionType { Id = 2, Descripcion = "Vacaciones" }
            }
        };

        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(permissions);
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);

        // Act
        var result = await _handler.Handle();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Juan", result[0].NombreEmpleado);
        Assert.Equal("María", result[1].NombreEmpleado);
        _kafkaMock.Verify(x => x.PublishOperationAsync(OperationType.Get), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoPermissions_ReturnsEmptyList()
    {
        // Arrange
        var permissionRepoMock = new Mock<IPermissionRepository>();
        permissionRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Permission>());
        _unitOfWorkMock.Setup(x => x.Permissions).Returns(permissionRepoMock.Object);

        // Act
        var result = await _handler.Handle();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _kafkaMock.Verify(x => x.PublishOperationAsync(OperationType.Get), Times.Once);
    }
}

