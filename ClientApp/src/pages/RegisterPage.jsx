import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { register } from '../api/auth'

const RegisterPage = () => {
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '', confirmPassword: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')

    if (form.password !== form.confirmPassword) {
      setError('Паролі не співпадають')
      return
    }
    if (form.password.length < 6) {
      setError('Пароль має бути мінімум 6 символів')
      return
    }

    setLoading(true)
    try {
      await register({ email: form.email, password: form.password })
      navigate('/login')
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка реєстрації')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card shadow p-4" style={{ width: '100%', maxWidth: 420 }}>
        <h3 className="text-center mb-4 fw-bold">💰 FinanceApp</h3>
        <h5 className="text-center text-muted mb-4">Реєстрація</h5>
        {error && <div className="alert alert-danger">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label">Email</label>
            <input
              type="email"
              className="form-control"
              value={form.email}
              onChange={e => setForm({ ...form, email: e.target.value })}
              required
              placeholder="your@email.com"
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              className="form-control"
              value={form.password}
              onChange={e => setForm({ ...form, password: e.target.value })}
              required
              placeholder="••••••••"
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Підтвердити пароль</label>
            <input
              type="password"
              className="form-control"
              value={form.confirmPassword}
              onChange={e => setForm({ ...form, confirmPassword: e.target.value })}
              required
              placeholder="••••••••"
            />
          </div>
          <button
            type="submit"
            className="btn btn-success w-100 mt-2"
            disabled={loading}
          >
            {loading ? 'Реєструємо...' : 'Зареєструватись'}
          </button>
          <div className="text-center mt-3">
            <span className="text-muted">Вже є акаунт? </span>
            <Link to="/login">Увійти</Link>
          </div>
        </form>
      </div>
    </div>
  )
}

export default RegisterPage