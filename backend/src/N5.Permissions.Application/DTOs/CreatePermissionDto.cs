using System.ComponentModel.DataAnnotations;

namespace N5.Permissions.Application.DTOs;

public class CreatePermissionDto
{
    [Required(ErrorMessage = "El nombre del empleado es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string NombreEmpleado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido del empleado es requerido")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string ApellidoEmpleado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de permiso es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de permiso debe ser mayor a 0")]
    public int TipoPermiso { get; set; }

    [Required(ErrorMessage = "La fecha del permiso es requerida")]
    public DateTime FechaPermiso { get; set; }
}

