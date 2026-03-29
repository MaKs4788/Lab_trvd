import api from './axios'

export const getCategories = () => api.get('/category')
export const getCategory = (id) => api.get(`/category/${id}`)
export const createCategory = (data) => api.post('/category', data)
export const updateCategory = (id, data) => api.put(`/category/${id}`, data)
export const deleteCategory = (id) => api.delete(`/category/${id}`)