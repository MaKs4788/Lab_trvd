import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { register } from '../api/auth'

const RegisterPage = () => {
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '', confirmPassword: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const validate = () => {
    if (!form.email.trim()) return 'Введіть email'
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) return 'Невірний формат email'
    if (!form.password) return 'Введіть пароль'
    if (form.password.length < 6) return 'Пароль має бути мінімум 6 символів'
    if (!/[A-Z]/.test(form.password)) return 'Пароль має містити хоча б одну велику літеру'
    if (!/[0-9]/.test(form.password)) return 'Пароль має містити хоча б одну цифру'
    if (form.password !== form.confirmPassword) return 'Паролі не співпадають'
    return null
  }

  const handleSubmit = async () => {
    setError('')
    const validationError = validate()
    if (validationError) {
      setError(validationError)
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

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') handleSubmit()
  }

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card shadow p-4" style={{ width: '100%', maxWidth: 420 }}>
        <h3 className="text-center mb-4 fw-bold">Фінанси Сторожук</h3>
        <h5 className="text-center text-muted mb-4">Реєстрація</h5>

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
              value={form.email}
              onChange={e => setForm({ ...form, email: e.target.value })}
              onKeyDown={handleKeyDown}
              placeholder="your@email.com"
              autoComplete="off"
            />
          </div>

          <div className="mb-3">
            <label className="form-label">Пароль</label>
            <input
              type="password"
              className="form-control"
              value={form.password}
              onChange={e => setForm({ ...form, password: e.target.value })}
              onKeyDown={handleKeyDown}
              placeholder="••••••••"
              autoComplete="off"
            />
            <div className="form-text text-muted">
              Мінімум 6 символів, одна велика літера та цифра
            </div>
          </div>

          <div className="mb-3">
            <label className="form-label">Підтвердити пароль</label>
            <input
              type="password"
              className="form-control"
              value={form.confirmPassword}
              onChange={e => setForm({ ...form, confirmPassword: e.target.value })}
              onKeyDown={handleKeyDown}
              placeholder="••••••••"
              autoComplete="off"
            />
          </div>

          <button
            type="button"
            className="btn btn-success w-100 mt-2"
            onClick={handleSubmit}
            disabled={loading}
          >
            {loading
              ? <><span className="spinner-border spinner-border-sm me-2" />Реєструємо...</>
              : 'Зареєструватись'
            }
          </button>
        </form>

        <div className="text-center mt-3">
          <span className="text-muted">Вже є акаунт? </span>
          <Link to="/login">Увійти</Link>
        </div>
      </div>
    </div>
  )
}

export default RegisterPage