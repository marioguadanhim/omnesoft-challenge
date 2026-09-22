export const getErrorMessage = (error, fallback = 'Something went wrong. Please try again.') => {
  const data = error?.response?.data;

  if (data?.errors) {
    const firstError = Object.values(data.errors).flat()[0];
    if (firstError) return firstError;
  }

  if (data?.errorDescription) {
    return String(data.errorDescription).replace('This is a Handled Error! | ', '');
  }

  if (data?.title) return data.title;

  if (error?.request) return 'Cannot connect to server. Please check your connection.';

  return fallback;
};
