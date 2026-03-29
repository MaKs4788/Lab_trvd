import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const Navbar = () => {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const isActive = (path) =>
    location.pathname === path ? 'nav-link active fw-semibold' : 'nav-link'

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark px-3">
      <Link className="navbar-brand fw-bold" to="/dashboard">Лабораторна ТРВД | Фінанси</Link>
      <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
        <span className="navbar-toggler-icon" />
      </button>
      <div className="collapse navbar-collapse" id="navMenu">
        <ul className="navbar-nav me-auto">
          <li className="nav-item">
            <Link className={isActive('/dashboard')} to="/dashboard">Дашборд</Link>
          </li>
          <li className="nav-item">
            <Link className={isActive('/budgets')} to="/budgets">Бюджети</Link>
          </li>
          <li className="nav-item">
            <Link className={isActive('/expenses')} to="/expenses">Витрати</Link>
          </li>
          <li className="nav-item">
            <Link className={isActive('/incomes')} to="/incomes">Доходи</Link>
          </li>
          <li className="nav-item">
            <Link className={isActive('/categories')} to="/categories">Категорії</Link>
          </li>
       {user?.role === 'Admin' && (
  <>
    <li className="nav-item">
      <Link className={isActive('/admin')} to="/admin">Адмін-панель</Link>
    </li>
    <li className="nav-item">
      <Link className={isActive('/admin/settings')} to="/admin/settings">Статистика</Link>
    </li>
  </>
)}
        </ul>
        <ul className="navbar-nav">
          <li className="nav-item">
            <span className="nav-link text-secondary">{user?.email}</span>
          </li>
          <li className="nav-item">
            <button className="btn btn-outline-danger btn-sm ms-2" onClick={handleLogout}>
              Вийти
            </button>
          </li>
        </ul>
      </div>
    </nav>
  )
}

export default Navbar