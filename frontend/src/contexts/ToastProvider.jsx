import React, { useState, useCallback, useMemo } from 'react';
import { Snackbar, Alert } from '@mui/material';
import { ToastContext } from './ToastContext';

function ToastProvider({ children }) {
  const [toast, setToast] = useState({ open: false, message: '', severity: 'error' });

  const show = useCallback((message, severity) => {
    setToast({ open: true, message, severity });
  }, []);

  const showError = useCallback((message) => show(message, 'error'), [show]);
  const showSuccess = useCallback((message) => show(message, 'success'), [show]);
  const showInfo = useCallback((message) => show(message, 'info'), [show]);

  const handleClose = (_, reason) => {
    if (reason === 'clickaway') return;
    setToast((prev) => ({ ...prev, open: false }));
  };

  const value = useMemo(
    () => ({ showError, showSuccess, showInfo }),
    [showError, showSuccess, showInfo]
  );

  return (
    <ToastContext.Provider value={value}>
      {children}
      <Snackbar
        open={toast.open}
        autoHideDuration={5000}
        onClose={handleClose}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert onClose={handleClose} severity={toast.severity} variant="filled" sx={{ width: '100%' }}>
          {toast.message}
        </Alert>
      </Snackbar>
    </ToastContext.Provider>
  );
}

export default ToastProvider;
