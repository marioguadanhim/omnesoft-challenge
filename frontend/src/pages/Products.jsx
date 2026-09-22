import React, { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  CircularProgress,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import RefreshIcon from '@mui/icons-material/Refresh';
import ProductFormDialog from '../components/ProductFormDialog';
import ConfirmDialog from '../components/ConfirmDialog';
import { useToast } from '../contexts/ToastContext';
import { isAdmin } from '../utils/jwtDecoder';
import {
  createProduct,
  deleteProduct,
  getProducts,
  updateProduct,
} from '../services/productService';

const currency = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

function Products() {
  const { showError, showSuccess } = useToast();

  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const [formOpen, setFormOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [productToDelete, setProductToDelete] = useState(null);
  const [formKey, setFormKey] = useState(0);

  const canManage = isAdmin();

  const applyResult = useCallback(
    (result) => {
      setLoading(false);
      if (result.success) {
        setProducts(result.data);
      } else {
        showError(result.error);
      }
    },
    [showError]
  );

  const loadProducts = useCallback(async () => {
    setLoading(true);
    applyResult(await getProducts());
  }, [applyResult]);

  useEffect(() => {
    let cancelled = false;

    (async () => {
      const result = await getProducts();
      if (!cancelled) applyResult(result);
    })();

    return () => {
      cancelled = true;
    };
  }, [applyResult]);

  const handleOpenCreate = () => {
    setEditingProduct(null);
    setFormKey((key) => key + 1);
    setFormOpen(true);
  };

  const handleOpenEdit = (product) => {
    setEditingProduct(product);
    setFormKey((key) => key + 1);
    setFormOpen(true);
  };

  const handleSave = async (values) => {
    setSaving(true);
    const result = editingProduct
      ? await updateProduct(editingProduct.id, values)
      : await createProduct(values);
    setSaving(false);

    if (!result.success) {
      showError(result.error);
      return;
    }

    showSuccess(editingProduct ? 'Product updated.' : 'Product created.');
    setFormOpen(false);
    setEditingProduct(null);
    loadProducts();
  };

  const handleDelete = async () => {
    if (!productToDelete) return;

    setDeleting(true);
    const result = await deleteProduct(productToDelete.id);
    setDeleting(false);

    if (!result.success) {
      showError(result.error);
      return;
    }

    showSuccess('Product deleted.');
    setProductToDelete(null);
    loadProducts();
  };

  return (
    <Box>
      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        justifyContent="space-between"
        alignItems={{ xs: 'stretch', sm: 'center' }}
        spacing={2}
        sx={{ mb: 3 }}
      >
        <Box>
          <Typography variant="h5" component="h1" fontWeight="bold">
            Products
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Manage the product catalogue.
          </Typography>
        </Box>

        <Stack direction="row" spacing={1}>
          <Button startIcon={<RefreshIcon />} onClick={loadProducts} disabled={loading}>
            Refresh
          </Button>
          {canManage && (
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>
              New product
            </Button>
          )}
        </Stack>
      </Stack>

      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell width={80}>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell align="right" width={140}>
                Price
              </TableCell>
              <TableCell>Description</TableCell>
              {canManage && (
                <TableCell align="right" width={120}>
                  Actions
                </TableCell>
              )}
            </TableRow>
          </TableHead>

          <TableBody>
            {loading && (
              <TableRow>
                <TableCell colSpan={canManage ? 5 : 4} align="center" sx={{ py: 6 }}>
                  <CircularProgress size={28} />
                </TableCell>
              </TableRow>
            )}

            {!loading && products.length === 0 && (
              <TableRow>
                <TableCell colSpan={canManage ? 5 : 4} align="center" sx={{ py: 6 }}>
                  <Typography color="text.secondary">
                    No products yet. Create the first one to get started.
                  </Typography>
                </TableCell>
              </TableRow>
            )}

            {!loading &&
              products.map((product) => (
                <TableRow key={product.id} hover>
                  <TableCell>{product.id}</TableCell>
                  <TableCell sx={{ fontWeight: 500 }}>{product.name}</TableCell>
                  <TableCell align="right">{currency.format(product.price)}</TableCell>
                  <TableCell sx={{ color: 'text.secondary' }}>{product.description || '—'}</TableCell>
                  {canManage && (
                    <TableCell align="right">
                      <Tooltip title="Edit">
                        <IconButton size="small" onClick={() => handleOpenEdit(product)}>
                          <EditIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete">
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => setProductToDelete(product)}
                        >
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </TableCell>
                  )}
                </TableRow>
              ))}
          </TableBody>
        </Table>
      </TableContainer>

      <ProductFormDialog
        key={formKey}
        open={formOpen}
        product={editingProduct}
        saving={saving}
        onSave={handleSave}
        onClose={() => {
          setFormOpen(false);
          setEditingProduct(null);
        }}
      />

      <ConfirmDialog
        open={Boolean(productToDelete)}
        title="Delete product"
        message={`Delete "${productToDelete?.name}"? This cannot be undone.`}
        confirmLabel="Delete"
        loading={deleting}
        onConfirm={handleDelete}
        onClose={() => setProductToDelete(null)}
      />
    </Box>
  );
}

export default Products;
