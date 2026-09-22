import React, { useState } from 'react';
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from '@mui/material';

const EMPTY_FORM = { name: '', price: '', description: '' };

const validate = ({ name, price }) => {
  const errors = {};

  if (!name.trim()) {
    errors.name = 'Name is required.';
  } else if (name.trim().length > 200) {
    errors.name = 'Name maximum size is 200.';
  }

  const parsedPrice = Number(price);
  if (price === '' || Number.isNaN(parsedPrice)) {
    errors.price = 'Price is required.';
  } else if (parsedPrice <= 0) {
    errors.price = 'Price must be greater than 0.';
  }

  return errors;
};

const toFormData = (product) =>
  product
    ? {
        name: product.name ?? '',
        price: product.price != null ? String(product.price) : '',
        description: product.description ?? '',
      }
    : EMPTY_FORM;

function ProductFormDialog({ open, product, saving, onSave, onClose }) {
  const [formData, setFormData] = useState(() => toFormData(product));
  const [errors, setErrors] = useState({});

  const isEdit = Boolean(product);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const validationErrors = validate(formData);
    setErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    onSave({
      name: formData.name.trim(),
      price: Number(formData.price),
      description: formData.description.trim() || null,
    });
  };

  return (
    <Dialog open={open} onClose={saving ? undefined : onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>{isEdit ? 'Edit product' : 'New product'}</DialogTitle>

        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              fullWidth
              label="Name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              error={Boolean(errors.name)}
              helperText={errors.name}
              disabled={saving}
              autoFocus
            />
            <TextField
              fullWidth
              label="Price"
              name="price"
              type="number"
              value={formData.price}
              onChange={handleChange}
              error={Boolean(errors.price)}
              helperText={errors.price}
              disabled={saving}
              slotProps={{ htmlInput: { step: '0.01', min: '0' } }}
            />
            <TextField
              fullWidth
              label="Description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              multiline
              minRows={3}
              disabled={saving}
            />
          </Stack>
        </DialogContent>

        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={onClose} disabled={saving}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" disabled={saving}>
            {saving ? <CircularProgress size={22} color="inherit" /> : isEdit ? 'Save changes' : 'Create'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}

export default ProductFormDialog;
