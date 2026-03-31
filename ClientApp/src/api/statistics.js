import api from './axios'

export const getStatsDashboard = () => api.get('/admin/statistics/dashboard')
export const getMonthlySummary = () => api.get('/admin/statistics/monthly-summary')
export const getTopCategories = (limit = 10) => api.get(`/admin/statistics/top-expense-categories?limit=${limit}`)
export const getUserStatistics = () => api.get('/admin/statistics/user-statistics')
export const exportReport = () => api.get('/admin/statistics/export-report')