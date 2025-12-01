export interface Permission {
  id: number;
  nombreEmpleado: string;
  apellidoEmpleado: string;
  tipoPermiso: number;
  tipoPermisoDescripcion?: string;
  fechaPermiso: string;
}

export interface CreatePermission {
  nombreEmpleado: string;
  apellidoEmpleado: string;
  tipoPermiso: number;
  fechaPermiso: string;
}

export interface UpdatePermission {
  nombreEmpleado?: string;
  apellidoEmpleado?: string;
  tipoPermiso?: number;
  fechaPermiso?: string;
}
