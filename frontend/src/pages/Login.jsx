import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Container,
  Paper,
  TextField,
  Typography,
} from '@mui/material';
import Inventory2Icon from '@mui/icons-material/Inventory2';
import { login } from '../services/authService';
import { isAuthenticated } from '../utils/auth';
import { useToast } from '../contexts/ToastContext';

function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const { showError } = useToast();

  const [formData, setFormData] = useState({ userName: '', password: '' });
  const [loading, setLoading] = useState(false);

  const redirectTo = location.state?.from?.pathname || '/products';

  useEffect(() => {
    if (isAuthenticated()) navigate(redirectTo, { replace: true });
  }, [navigate, redirectTo]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.userName || !formData.password) {
      showError('Please fill in your username and password');
      return;
    }

    setLoading(true);
    const result = await login(formData.userName, formData.password);
    setLoading(false);

    if (result.success) {
      navigate(redirectTo, { replace: true });
    } else {
      showError(result.error || 'Login failed');
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          py: 4,
        }}
      >
        <Paper elevation={3} sx={{ p: 4, width: '100%', px: { xs: 2, sm: 4 } }}>
          <Box sx={{ display: 'flex', justifyContent: 'center', mb: 1 }}>
            <Inventory2Icon color="primary" sx={{ fontSize: 40 }} />
          </Box>

          <Typography variant="h4" component="h1" gutterBottom align="center" sx={{ mb: 1 }}>
            Omnesoft Challenge
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
            Welcome! Please log in to continue.
          </Typography>

          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Username"
              name="userName"
              value={formData.userName}
              onChange={handleChange}
              margin="normal"
              required
              autoComplete="username"
              autoFocus
              disabled={loading}
            />
            <TextField
              fullWidth
              label="Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              margin="normal"
              required
              autoComplete="current-password"
              disabled={loading}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              size="large"
              disabled={loading}
              sx={{ mt: 3, mb: 1 }}
            >
              {loading ? <CircularProgress size={24} color="inherit" /> : 'Sign In'}
            </Button>
          </form>

          <Alert severity="info" sx={{ mt: 2 }}>
            Use one of these accounts:
            <br />
            Admin: <strong>admin</strong> / <strong>admin123</strong>
            <br />
            Guest: <strong>guest</strong> / <strong>guest123</strong>
          </Alert>
        </Paper>
      </Box>
    </Container>
  );
}

export default Login;
