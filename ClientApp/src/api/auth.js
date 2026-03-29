import api from './axios'

export const signIn = (data) => api.post('/auth/login', data)
export const refreshToken = (data) => api.post('/auth/refresh', data)
export const revokeToken = (data) => api.post('/auth/revoke', data)
export const register = (data) => api.post('/auth/register', data)