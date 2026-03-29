import api from './axios'

export const getBudgetSummary = (month, year) => 
  api.get(`/budget/summary?month=${month}&year=${year}`)
export const createBudget = (data) => api.post('/budget', data)