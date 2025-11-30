using System.ComponentModel.DataAnnotations;

namespace N5.Permissions.Application.DTOs;

public class UpdatePermissionDto
{
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? NombreEmpleado { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? ApellidoEmpleado { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El tipo de permiso debe ser mayor a 0")]
    public int? TipoPermiso { get; set; }

    public DateTime? FechaPermiso { get; set; }
}

