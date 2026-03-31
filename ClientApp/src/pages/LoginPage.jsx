import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const LoginPage = () => {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async () => {
    setError('')

    if (!email.trim()) { setError('Введіть email'); return }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { setError('Невірний формат email'); return }
    if (!password.trim()) { setError('Введіть пароль'); return }

    setLoading(true)
  try {
  await login(email, password)
  navigate('/dashboard')
} catch (err) {
  console.log('err:', err)
  console.log('err.response:', err.response)
  console.log('err.response?.data:', err.response?.data)
  setError(err.response?.data?.message || 'Невірний email або пароль')
} finally {
  setLoading(false)
}
  } 

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') handleSubmit()
  }

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card shadow p-4" style={{ width: '100%', maxWidth: 420 }}>
        <h3 className="text-center mb-4 fw-bold">Фінанси Сторожук</h3>
        <h5 className="text-center text-muted mb-4">Вхід в систему</h5>

        {error && (
          <div className="alert alert-danger d-flex justify-content-between align-items-center">
            <span>{error}</span>
            <button type="button" className="btn-close" onClick={() => setError('')} />
          </div>
        )}

        <form onSubmit={e => e.preventDefault()}>
          <div className="mb-3">
            <label className="form-label">Email</label>
            <input
              type="text"
              className="form-control"
              value={email}
              onChange={e => setEmail(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="test@example.com"
              autoComplete="off"
            />
          </div>

          <div className="mb-3">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              className="form-control"
              value={password}
              onChange={e => setPassword(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="••••••••"
              autoComplete="off"
            />
          </div>

          <button
            type="button"
            className="btn btn-primary w-100 mt-2"
            onClick={handleSubmit}
            disabled={loading}
          >
            {loading
              ? <><span className="spinner-border spinner-border-sm me-2" />Входимо...</>
              : 'Увійти'
            }
          </button>
        </form>

        <div className="text-center mt-3">
          <span className="text-muted">Немає акаунту? </span>
          <Link to="/register">Зареєструватись</Link>
        </div>
      </div>
    </div>
  )
}

export default LoginPage