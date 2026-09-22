import apiClient from './api';
import { getErrorMessage } from '../utils/apiError';

export const getProducts = async () => {
  try {
    const response = await apiClient.get('/products');
    return { success: true, data: response.data.products ?? [] };
  } catch (error) {
    return { success: false, error: getErrorMessage(error, 'Failed to load products.') };
  }
};

export const getProductById = async (id) => {
  try {
    const response = await apiClient.get(`/products/${id}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error, 'Failed to load the product.') };
  }
};

export const createProduct = async (product) => {
  try {
    const response = await apiClient.post('/products', product);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error, 'Failed to create the product.') };
  }
};

export const updateProduct = async (id, product) => {
  try {
    const response = await apiClient.put(`/products/${id}`, product);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error, 'Failed to update the product.') };
  }
};

export const deleteProduct = async (id) => {
  try {
    const response = await apiClient.delete(`/products/${id}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { success: false, error: getErrorMessage(error, 'Failed to delete the product.') };
  }
};
