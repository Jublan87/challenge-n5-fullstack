import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box,
  Typography,
  IconButton,
  InputAdornment,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import CloseIcon from '@mui/icons-material/Close';
import PersonOutlineIcon from '@mui/icons-material/PersonOutline';
import BadgeOutlinedIcon from '@mui/icons-material/BadgeOutlined';
import dayjs, { type Dayjs } from 'dayjs';
import 'dayjs/locale/es';
import type {
  Permission,
  CreatePermission,
  UpdatePermission,
} from '../types/permission';

interface PermissionFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreatePermission | UpdatePermission) => Promise<void>;
  permission?: Permission | null;
}

// En una aplicación completa, estos tipos se obtendrían
// de un endpoint del backend (GET /api/permission-types)
const PERMISSION_TYPES = [
  { id: 1, descripcion: 'Enfermedad', icon: '🏥' },
  { id: 2, descripcion: 'Tramite', icon: '📋' },
  { id: 3, descripcion: 'Mudanza', icon: '🏠' },
  { id: 4, descripcion: 'Vacaciones', icon: '🏖️' },
];

interface FormData {
  nombreEmpleado: string;
  apellidoEmpleado: string;
  tipoPermiso: number;
  fechaPermiso: Dayjs;
}

const getInitialFormData = (
  permission: Permission | null | undefined
): FormData => ({
  nombreEmpleado: permission?.nombreEmpleado ?? '',
  apellidoEmpleado: permission?.apellidoEmpleado ?? '',
  tipoPermiso: permission?.tipoPermiso ?? 1,
  fechaPermiso: permission ? dayjs(permission.fechaPermiso) : dayjs(),
});

const PermissionForm = ({
  open,
  onClose,
  onSubmit,
  permission,
}: PermissionFormProps) => {
  const [isEdit, setIsEdit] = useState(!!permission);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState(getInitialFormData(permission));

  useEffect(() => {
    if (open) {
      const editMode = !!permission;
      setIsEdit(editMode);
      setFormData(getInitialFormData(permission));
    }
  }, [permission, open]);

  const handleChange = (
    field: keyof Omit<FormData, 'fechaPermiso'>,
    value: string | number
  ) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async () => {
    setLoading(true);
    try {
      const submitData = {
        nombreEmpleado: formData.nombreEmpleado,
        apellidoEmpleado: formData.apellidoEmpleado,
        tipoPermiso: formData.tipoPermiso,
        fechaPermiso: formData.fechaPermiso.toISOString(),
      };

      if (isEdit) {
        await onSubmit(submitData as UpdatePermission);
      } else {
        await onSubmit(submitData as CreatePermission);
      }
      onClose();
    } catch (error) {
      console.error('Error al guardar permiso:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="es">
      <Dialog
        open={open}
        onClose={onClose}
        maxWidth="sm"
        fullWidth
        PaperProps={{
          sx: { borderRadius: 3 },
        }}
      >
        <DialogTitle
          sx={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            pb: 1,
          }}
        >
          <Box>
            <Typography variant="h6" fontWeight={700}>
              {isEdit ? 'Editar Permiso' : 'Nuevo Permiso'}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {isEdit
                ? 'Modifica los datos del permiso'
                : 'Completa los datos para crear un nuevo permiso'}
            </Typography>
          </Box>
          <IconButton
            onClick={onClose}
            size="small"
            sx={{ color: 'text.secondary' }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>

        <DialogContent sx={{ pt: 2 }}>
          <Box
            sx={{ display: 'flex', flexDirection: 'column', gap: 2.5, mt: 1 }}
          >
            <TextField
              label="Nombre del Empleado"
              value={formData.nombreEmpleado}
              onChange={(e) => handleChange('nombreEmpleado', e.target.value)}
              fullWidth
              required
              disabled={loading}
              placeholder="Ej: Juan"
              InputLabelProps={{ shrink: true }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <PersonOutlineIcon sx={{ color: 'text.secondary' }} />
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              label="Apellido del Empleado"
              value={formData.apellidoEmpleado}
              onChange={(e) => handleChange('apellidoEmpleado', e.target.value)}
              fullWidth
              required
              disabled={loading}
              placeholder="Ej: Pérez"
              InputLabelProps={{ shrink: true }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <BadgeOutlinedIcon sx={{ color: 'text.secondary' }} />
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              select
              label="Tipo de Permiso"
              value={formData.tipoPermiso}
              onChange={(e) =>
                handleChange('tipoPermiso', Number(e.target.value))
              }
              fullWidth
              required
              disabled={loading}
            >
              {PERMISSION_TYPES.map((type) => (
                <MenuItem key={type.id} value={type.id}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                    <span>{type.icon}</span>
                    <span>{type.descripcion}</span>
                  </Box>
                </MenuItem>
              ))}
            </TextField>
            <DatePicker
              label="Fecha del Permiso"
              value={formData.fechaPermiso}
              onChange={(newValue: Dayjs | null) => {
                if (newValue) {
                  setFormData((prev) => ({ ...prev, fechaPermiso: newValue }));
                }
              }}
              slotProps={{
                textField: {
                  fullWidth: true,
                  required: true,
                  disabled: loading,
                },
              }}
            />
          </Box>
        </DialogContent>

        <DialogActions sx={{ px: 3, pb: 3, pt: 1 }}>
          <Button
            onClick={onClose}
            disabled={loading}
            sx={{ color: 'text.secondary' }}
          >
            Cancelar
          </Button>
          <Button
            onClick={handleSubmit}
            variant="contained"
            disabled={
              loading || !formData.nombreEmpleado || !formData.apellidoEmpleado
            }
            sx={{
              minWidth: 140,
              background: 'linear-gradient(135deg, #0f172a 0%, #334155 100%)',
              color: '#ffffff',
              '&:hover': {
                background: 'linear-gradient(135deg, #1e293b 0%, #475569 100%)',
              },
              '&.Mui-disabled': {
                background: '#e2e8f0',
                color: '#94a3b8',
              },
            }}
          >
            {loading
              ? 'Guardando...'
              : isEdit
              ? 'Guardar Cambios'
              : 'Crear Permiso'}
          </Button>
        </DialogActions>
      </Dialog>
    </LocalizationProvider>
  );
};

export default PermissionForm;
