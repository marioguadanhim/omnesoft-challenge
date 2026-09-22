const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const EXPIRES_AT_KEY = 'expiresAt';

export const setAuthData = (accessToken, refreshToken, expiresAt) => {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  localStorage.setItem(EXPIRES_AT_KEY, new Date(expiresAt).toISOString());
};

export const getAccessToken = () => localStorage.getItem(ACCESS_TOKEN_KEY);
export const getRefreshToken = () => localStorage.getItem(REFRESH_TOKEN_KEY);
export const getExpiresAt = () => localStorage.getItem(EXPIRES_AT_KEY);

export const clearAuthData = () => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(EXPIRES_AT_KEY);
};

export const isAuthenticated = () => {
  const token = getAccessToken();
  const expiresAt = getExpiresAt();

  if (!token) return false;

  if (expiresAt) {
    const expirationDate = new Date(expiresAt);
    if (!isNaN(expirationDate.getTime()) && new Date() >= expirationDate) {
      return false;
    }
  }

  return true;
};
