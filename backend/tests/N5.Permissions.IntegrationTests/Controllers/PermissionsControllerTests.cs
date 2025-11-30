using N5.Permissions.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace N5.Permissions.IntegrationTests.Controllers;

public class PermissionsControllerTests : IClassFixture<Helpers.CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PermissionsControllerTests(Helpers.CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_Permissions_WhenValidData_ReturnsCreatedPermission()
    {
        // Arrange
        var dto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/permissions", dto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(result);
        Assert.Equal(dto.NombreEmpleado, result.NombreEmpleado);
        Assert.Equal(dto.ApellidoEmpleado, result.ApellidoEmpleado);
        Assert.Equal(dto.TipoPermiso, result.TipoPermiso);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task POST_Permissions_WhenInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePermissionDto
        {
            NombreEmpleado = "",
            ApellidoEmpleado = "",
            TipoPermiso = 0,
            FechaPermiso = default
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/permissions", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Permissions_WhenPermissionTypeDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 999,
            FechaPermiso = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/permissions", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Permissions_WhenPermissionExists_ReturnsUpdatedPermission()
    {
        // Arrange - Primero crear un permiso
        var createDto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var createResponse = await _client.PostAsJsonAsync("/api/permissions", createDto);
        createResponse.EnsureSuccessStatusCode();
        var createdPermission = await createResponse.Content.ReadFromJsonAsync<PermissionDto>();

        // Act - Actualizar el permiso
        var updateDto = new UpdatePermissionDto
        {
            NombreEmpleado = "Pedro",
            ApellidoEmpleado = "López"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/permissions/{createdPermission!.Id}", updateDto);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedPermission = await updateResponse.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(updatedPermission);
        Assert.Equal("Pedro", updatedPermission.NombreEmpleado);
        Assert.Equal("López", updatedPermission.ApellidoEmpleado);
        Assert.Equal(createdPermission.Id, updatedPermission.Id);
    }

    [Fact]
    public async Task PUT_Permissions_WhenPermissionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdatePermissionDto
        {
            NombreEmpleado = "Pedro"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/permissions/999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Permissions_WhenUpdatingTipoPermiso_ValidatesPermissionTypeExists()
    {
        // Arrange - Crear un permiso
        var createDto = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var createResponse = await _client.PostAsJsonAsync("/api/permissions", createDto);
        createResponse.EnsureSuccessStatusCode();
        var createdPermission = await createResponse.Content.ReadFromJsonAsync<PermissionDto>();

        // Act - Actualizar con un tipo de permiso válido
        var updateDto = new UpdatePermissionDto
        {
            TipoPermiso = 2
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/permissions/{createdPermission!.Id}", updateDto);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        var updatedPermission = await updateResponse.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(updatedPermission);
        Assert.Equal(2, updatedPermission.TipoPermiso);
    }

    [Fact]
    public async Task GET_Permissions_ReturnsListOfPermissions()
    {
        // Arrange - Crear algunos permisos
        var dto1 = new CreatePermissionDto
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "García",
            TipoPermiso = 1,
            FechaPermiso = DateTime.Now
        };

        var dto2 = new CreatePermissionDto
        {
            NombreEmpleado = "María",
            ApellidoEmpleado = "López",
            TipoPermiso = 2,
            FechaPermiso = DateTime.Now
        };

        await _client.PostAsJsonAsync("/api/permissions", dto1);
        await _client.PostAsJsonAsync("/api/permissions", dto2);

        // Act
        var response = await _client.GetAsync("/api/permissions");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>();
        Assert.NotNull(permissions);
        Assert.True(permissions.Count >= 2);
    }

    [Fact]
    public async Task GET_Permissions_WhenNoPermissions_ReturnsEmptyList()
    {
        // Act - En una base de datos limpia
        var response = await _client.GetAsync("/api/permissions");

        // Assert
        response.EnsureSuccessStatusCode();
        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>();
        Assert.NotNull(permissions);
    }
}

