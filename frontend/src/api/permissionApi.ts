import apiClient from './axios';
import type {
  Permission,
  CreatePermission,
  UpdatePermission,
} from '../types/permission';

export const getPermissions = async (): Promise<Permission[]> => {
  const response = await apiClient.get<Permission[]>('/permissions');
  return response.data;
};

export const createPermission = async (
  data: CreatePermission
): Promise<Permission> => {
  const response = await apiClient.post<Permission>('/permissions', data);
  return response.data;
};

export const updatePermission = async (
  id: number,
  data: UpdatePermission
): Promise<Permission> => {
  const response = await apiClient.put<Permission>(`/permissions/${id}`, data);
  return response.data;
};
