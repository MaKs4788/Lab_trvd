import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const AdminRoute = ({ children }) => {
  const { user, loading } = useAuth()
  if (loading) return <div className="text-center mt-5">Завантаження...</div>
  if (!user) return <Navigate to="/login" />
  if (user.role !== 'Admin') return <Navigate to="/dashboard" />
  return children
}

export default AdminRoute