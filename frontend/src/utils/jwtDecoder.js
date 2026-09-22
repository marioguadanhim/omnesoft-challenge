import { getAccessToken } from './auth';

const ROLE_CLAIM_URI = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAME_CLAIM_URI = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

export const decodeJwtToken = (token) => {
  if (!token) return null;

  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error decoding JWT token:', error);
    return null;
  }
};

export const getCurrentUser = () => {
  const decoded = decodeJwtToken(getAccessToken());
  if (!decoded) return null;

  return {
    id: decoded.nameid || decoded.sub,
    username: decoded.unique_name || decoded.name || decoded[NAME_CLAIM_URI],
    role: decoded.role || decoded[ROLE_CLAIM_URI],
    exp: decoded.exp,
  };
};

export const getUsername = () => getCurrentUser()?.username ?? null;

export const getUserRole = () => getCurrentUser()?.role ?? null;

export const isAdmin = () => getUserRole() === 'Admin';
