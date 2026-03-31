import api from './axios'

export const getUsers = () => api.get('/admin/users')
export const blockUser = (id) => api.put(`/admin/users/${id}/block`)
export const unblockUser = (id) => api.put(`/admin/users/${id}/unblock`)
export const deleteUser = (id) => api.delete(`/admin/users/${id}`)
export const changeUserRole = (id, role) => api.put(`/admin/users/${id}/role`, { newRole: role })