import api from './axios'

export const getIncomes = () => api.get('/income')
export const getIncome = (id) => api.get(`/income/${id}`)
export const createIncome = (data) => api.post('/income', data)
export const updateIncome = (id, data) => api.put(`/income/${id}`, data)
export const deleteIncome = (id) => api.delete(`/income/${id}`)