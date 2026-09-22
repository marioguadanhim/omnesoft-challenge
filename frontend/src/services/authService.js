import apiClient from './api';
import { clearAuthData, setAuthData } from '../utils/auth';
import { getErrorMessage } from '../utils/apiError';

export const login = async (userName, password) => {
  try {
    const response = await apiClient.post('/Auth/login', { userName, password });

    const { accessToken, refreshToken, expiresAt } = response.data;

    if (accessToken && refreshToken) {
      setAuthData(accessToken, refreshToken, expiresAt);
      return { success: true, data: response.data };
    }

    return { success: false, error: 'No tokens received from server' };
  } catch (error) {
    if (error.response?.status === 401) {
      return { success: false, error: getErrorMessage(error, 'Invalid username or password') };
    }
    return { success: false, error: getErrorMessage(error, 'Login failed. Please try again.') };
  }
};

export const logout = () => clearAuthData();
