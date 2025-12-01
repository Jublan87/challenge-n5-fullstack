import { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  AppBar,
  Toolbar,
  Paper,
  Fade,
  Snackbar,
  Alert,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import AssignmentIcon from '@mui/icons-material/Assignment';
import PermissionTable from './components/PermissionTable';
import PermissionForm from './components/PermissionForm';
import {
  getPermissions,
  createPermission,
  updatePermission,
} from './api/permissionApi';
import type {
  Permission,
  CreatePermission,
  UpdatePermission,
} from './types/permission';

interface SnackbarState {
  open: boolean;
  message: string;
  severity: 'success' | 'error';
}

function App() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPermission, setEditingPermission] = useState<Permission | null>(
    null
  );
  const [snackbar, setSnackbar] = useState<SnackbarState>({
    open: false,
    message: '',
    severity: 'success',
  });

  const showMessage = (message: string, severity: 'success' | 'error') => {
    setSnackbar({ open: true, message, severity });
  };

  const loadPermissions = async () => {
    try {
      setLoading(true);
      const data = await getPermissions();
      setPermissions(data);
    } catch {
      showMessage('Error al cargar los permisos', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPermissions();
  }, []);

  const handleCreate = () => {
    setEditingPermission(null);
    setFormOpen(true);
  };

  const handleEdit = (permission: Permission) => {
    setEditingPermission(permission);
    setFormOpen(true);
  };

  const handleFormClose = () => {
    setFormOpen(false);
    setEditingPermission(null);
  };

  const handleFormSubmit = async (
    data: CreatePermission | UpdatePermission
  ) => {
    try {
      if (editingPermission) {
        await updatePermission(editingPermission.id, data as UpdatePermission);
        showMessage('Permiso actualizado correctamente', 'success');
      } else {
        await createPermission(data as CreatePermission);
        showMessage('Permiso creado correctamente', 'success');
      }
      await loadPermissions();
    } catch {
      showMessage('Error al guardar el permiso', 'error');
      throw new Error('Error al guardar');
    }
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" elevation={0}>
        <Toolbar sx={{ py: 1 }}>
          <AssignmentIcon sx={{ mr: 2, fontSize: 28 }} />
          <Typography
            variant="h6"
            component="div"
            sx={{ flexGrow: 1, fontWeight: 700, letterSpacing: '-0.01em' }}
          >
            N5 Permissions
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ flex: 1, py: 4 }}>
        <Fade in timeout={500}>
          <Box>
            <Box
              sx={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                mb: 4,
              }}
            >
              <Box>
                <Typography
                  variant="h4"
                  component="h1"
                  sx={{ color: 'text.primary', mb: 0.5 }}
                >
                  Gestión de Permisos
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  Administra los permisos de los empleados
                </Typography>
              </Box>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={handleCreate}
                size="large"
                sx={{
                  background:
                    'linear-gradient(135deg, #0f172a 0%, #334155 100%)',
                  '&:hover': {
                    background:
                      'linear-gradient(135deg, #1e293b 0%, #475569 100%)',
                  },
                }}
              >
                Nuevo Permiso
              </Button>
            </Box>

            <Paper
              sx={{
                borderRadius: 3,
                overflow: 'hidden',
                border: '1px solid',
                borderColor: 'divider',
              }}
            >
              <PermissionTable
                permissions={permissions}
                loading={loading}
                onEdit={handleEdit}
              />
            </Paper>
          </Box>
        </Fade>

        <PermissionForm
          open={formOpen}
          onClose={handleFormClose}
          onSubmit={handleFormSubmit}
          permission={editingPermission}
        />
      </Container>

      <AppBar
        position="static"
        component="footer"
        elevation={0}
        sx={{ top: 'auto', bottom: 0 }}
      >
        <Toolbar sx={{ py: 1, justifyContent: 'center' }}>
          <Typography variant="body2" sx={{ fontWeight: 500 }}>
            N5 Challenge © {new Date().getFullYear()}
          </Typography>
        </Toolbar>
      </AppBar>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
          severity={snackbar.severity}
          variant="filled"
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

export default App;
