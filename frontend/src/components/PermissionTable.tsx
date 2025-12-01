import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  CircularProgress,
  Box,
  Typography,
  Chip,
  Tooltip,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import AssignmentIcon from '@mui/icons-material/Assignment';
import type { Permission } from '../types/permission';

interface PermissionTableProps {
  permissions: Permission[];
  loading: boolean;
  onEdit: (permission: Permission) => void;
}

const PERMISSION_COLORS: Record<number, { bg: string; text: string }> = {
  1: { bg: '#fef2f2', text: '#dc2626' }, // Enfermedad - rojo
  2: { bg: '#eff6ff', text: '#2563eb' }, // Tramite - azul
  3: { bg: '#f0fdf4', text: '#16a34a' }, // Mudanza - verde
  4: { bg: '#fefce8', text: '#ca8a04' }, // Vacaciones - amarillo
};

const PermissionTable = ({
  permissions,
  loading,
  onEdit,
}: PermissionTableProps) => {
  if (loading) {
    return (
      <Box
        display="flex"
        flexDirection="column"
        justifyContent="center"
        alignItems="center"
        py={8}
      >
        <CircularProgress size={48} sx={{ mb: 2 }} />
        <Typography color="text.secondary">Cargando permisos...</Typography>
      </Box>
    );
  }

  if (permissions.length === 0) {
    return (
      <Box py={8} textAlign="center">
        <Box
          sx={{
            width: 80,
            height: 80,
            borderRadius: '50%',
            backgroundColor: '#f1f5f9',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            margin: '0 auto 16px',
          }}
        >
          <AssignmentIcon sx={{ fontSize: 40, color: '#94a3b8' }} />
        </Box>
        <Typography variant="h6" color="text.primary" gutterBottom>
          No hay permisos registrados
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Crea un nuevo permiso para comenzar
        </Typography>
      </Box>
    );
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell>Empleado</TableCell>
            <TableCell>Tipo de Permiso</TableCell>
            <TableCell>Fecha</TableCell>
            <TableCell align="center" sx={{ width: 100 }}>
              Acciones
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {permissions.map((permission) => {
            const colors = PERMISSION_COLORS[permission.tipoPermiso] || {
              bg: '#f1f5f9',
              text: '#475569',
            };
            return (
              <TableRow key={permission.id}>
                <TableCell>
                  <Typography variant="body2" color="text.secondary">
                    {permission.id}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body1" fontWeight={500}>
                    {permission.nombreEmpleado} {permission.apellidoEmpleado}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={
                      permission.tipoPermisoDescripcion ||
                      `Tipo ${permission.tipoPermiso}`
                    }
                    size="small"
                    sx={{
                      backgroundColor: colors.bg,
                      color: colors.text,
                      fontWeight: 600,
                      border: 'none',
                    }}
                  />
                </TableCell>
                <TableCell>
                  <Typography variant="body2" color="text.secondary">
                    {formatDate(permission.fechaPermiso)}
                  </Typography>
                </TableCell>
                <TableCell align="center">
                  <Tooltip title="Editar permiso" arrow>
                    <IconButton
                      onClick={() => onEdit(permission)}
                      aria-label="editar"
                      sx={{
                        color: '#64748b',
                        backgroundColor: '#f1f5f9',
                        '&:hover': {
                          backgroundColor: '#e2e8f0',
                          color: '#0f172a',
                        },
                      }}
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default PermissionTable;
