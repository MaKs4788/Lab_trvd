import { createContext, useContext, useState, useEffect } from 'react'
import { signIn, revokeToken } from '../api/auth'

const AuthContext = createContext(null)

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('accessToken')
    const email = localStorage.getItem('email')
    const role = localStorage.getItem('role')
    if (token && email) {
      setUser({ email, role })
    }
    setLoading(false)
  }, [])

  const login = async (email, password) => {
    const { data } = await signIn({ email, password })
    localStorage.setItem('accessToken', data.token)
    localStorage.setItem('refreshToken', data.refreshToken)
    localStorage.setItem('email', data.email)
    localStorage.setItem('role', data.role)
    setUser({ email: data.email, role: data.role })
  }

  const logout = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken')
      if (refreshToken) await revokeToken({ refreshToken })
    } catch {}
    localStorage.clear()
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)