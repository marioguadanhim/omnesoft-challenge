import axios from 'axios';
import config from '../config/env';
import { clearAuthData, getAccessToken, getRefreshToken, setAuthData } from '../utils/auth';

const apiClient = axios.create({
  baseURL: config.apiBaseUrl,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use(
  (cfg) => {
    const token = getAccessToken();
    if (token) cfg.headers.Authorization = `Bearer ${token}`;
    return cfg;
  },
  (error) => Promise.reject(error)
);

let isRefreshing = false;
let pendingRequests = [];

const onRefreshed = (token) => {
  pendingRequests.forEach((callback) => callback(token));
  pendingRequests = [];
};

const onRefreshFailed = () => {
  pendingRequests.forEach((callback) => callback(null));
  pendingRequests = [];
};

const redirectToLogin = () => {
  clearAuthData();
  if (window.location.pathname !== '/login') {
    window.location.href = '/login';
  }
};

const refreshAccessToken = async () => {
  const storedRefreshToken = getRefreshToken();
  if (!storedRefreshToken) return null;

  const response = await axios.post(
    `${config.apiBaseUrl}/Auth/refresh`,
    { refreshToken: storedRefreshToken },
    { headers: { 'Content-Type': 'application/json' } }
  );

  const accessToken = response.data.accessToken;
  const newRefreshToken = response.data.refreshToken;
  const expiresAt = response.data.expiresAt;

  if (!accessToken || !newRefreshToken) return null;

  setAuthData(accessToken, newRefreshToken, expiresAt);
  return accessToken;
};

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status !== 401 || !originalRequest || originalRequest._retry) {
      if (error.response?.status === 401) redirectToLogin();
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        pendingRequests.push((token) => {
          if (!token) {
            reject(error);
            return;
          }
          originalRequest.headers.Authorization = `Bearer ${token}`;
          resolve(apiClient(originalRequest));
        });
      });
    }

    isRefreshing = true;

    try {
      const newToken = await refreshAccessToken();

      if (!newToken) {
        onRefreshFailed();
        redirectToLogin();
        return Promise.reject(error);
      }

      onRefreshed(newToken);
      originalRequest.headers.Authorization = `Bearer ${newToken}`;
      return apiClient(originalRequest);
    } catch (refreshError) {
      onRefreshFailed();
      redirectToLogin();
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);

export default apiClient;
