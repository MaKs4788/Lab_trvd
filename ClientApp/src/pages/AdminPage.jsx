import { useEffect, useState } from 'react'
import { getUsers, blockUser, unblockUser, deleteUser, changeUserRole } from '../api/admin'
import { getStatsDashboard, getMonthlySummary, getTopCategories, exportReport } from '../api/statistics'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts'
import Layout from '../components/Layout'

const COLORS = ['#0d6efd', '#198754', '#dc3545', '#ffc107', '#0dcaf0', '#6f42c1']

const AdminPage = () => {
  const [users, setUsers] = useState([])
  const [stats, setStats] = useState(null)
  const [monthly, setMonthly] = useState(null)
  const [topCategories, setTopCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [activeTab, setActiveTab] = useState('stats')
  const [error, setError] = useState('')

  const fetchData = async () => {
    try {
      const [u, s, m, t] = await Promise.all([
        getUsers(),
        getStatsDashboard(),
        getMonthlySummary(),
        getTopCategories()
      ])
      setUsers(u.data || [])
      setStats(s.data)
      setMonthly(m.data)
      setTopCategories(t.data || [])
    } catch (err) {
      setError('Помилка завантаження')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [])

  const handleBlock = async (id, isBlocked) => {
    try {
      if (isBlocked) await unblockUser(id)
      else await blockUser(id)
      fetchData()
    } catch { alert('Помилка зміни статусу') }
  }

  const handleDelete = async (id) => {
    if (!confirm('Видалити користувача? Це незворотня дія!')) return
    try {
      await deleteUser(id)
      fetchData()
    } catch { alert('Помилка видалення') }
  }

  const handleRoleChange = async (id, role) => {
    try {
      await changeUserRole(id, role)
      fetchData()
    } catch { alert('Помилка зміни ролі') }
  }

  const handleExport = async () => {
    try {
      const { data } = await exportReport()
      const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `report_${new Date().toISOString().split('T')[0]}.json`
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      alert('Помилка експорту')
    }
  }

  if (loading) return <Layout><div className="text-center mt-5"><div className="spinner-border text-primary" /></div></Layout>

  const barData = monthly ? [
    { name: 'Доходи', value: monthly.newIncomes },
    { name: 'Витрати', value: monthly.newExpenses }
  ] : []

  return (
    <Layout>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">⚙️ Адмін-панель</h4>
        <button className="btn btn-outline-success btn-sm" onClick={handleExport}>
          Експорт статстики JSON
        </button>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      <ul className="nav nav-tabs mb-4">
        <li className="nav-item">
          <button className={`nav-link ${activeTab === 'stats' ? 'active' : ''}`} onClick={() => setActiveTab('stats')}>
            📊 Статистика
          </button>
        </li>
        <li className="nav-item">
          <button className={`nav-link ${activeTab === 'users' ? 'active' : ''}`} onClick={() => setActiveTab('users')}>
            👥 Користувачі ({users.length})
          </button>
        </li>
      </ul>
      {activeTab === 'stats' && stats && (
        <>
          <div className="row g-3 mb-4">
            <div className="col-md-3">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Всього користувачів</div>
                <h3 className="fw-bold text-primary">{stats.totalUsers}</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Активних (30 днів)</div>
                <h3 className="fw-bold text-success">{stats.activeUsers}</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Заблокованих</div>
                <h3 className="fw-bold text-danger">{stats.blockedUsers}</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Транзакцій</div>
                <h3 className="fw-bold text-info">{stats.transactionStats?.totalTransactions}</h3>
              </div>
            </div>
            <div className="col-md-4">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Загальні доходи</div>
                <h4 className="fw-bold text-success">{stats.amountStats?.totalIncomeAmount?.toFixed(2)} ₴</h4>
              </div>
            </div>
            <div className="col-md-4">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Загальні витрати</div>
                <h4 className="fw-bold text-danger">{stats.amountStats?.totalExpenseAmount?.toFixed(2)} ₴</h4>
              </div>
            </div>
            <div className="col-md-4">
              <div className="card border-0 shadow-sm text-center p-3">
                <div className="text-muted small">Баланс системи</div>
                <h4 className={`fw-bold ${stats.amountStats?.balance >= 0 ? 'text-success' : 'text-danger'}`}>
                  {stats.amountStats?.balance?.toFixed(2)} ₴
                </h4>
              </div>
            </div>
          </div>
          <div className="row g-3">
            <div className="col-md-6">
              <div className="card border-0 shadow-sm p-3">
                <h6 className="text-muted mb-3">За останні 30 днів</h6>
                {monthly && (
                  <>
                    <div className="d-flex justify-content-between mb-3">
                      <span className="text-muted small">Нових користувачів: <strong>{monthly.newUsers}</strong></span>
                      <span className="text-muted small">Чистий потік: <strong className={monthly.netFlow >= 0 ? 'text-success' : 'text-danger'}>{monthly.netFlow?.toFixed(2)} ₴</strong></span>
                    </div>
                    <ResponsiveContainer width="100%" height={200}>
                      <BarChart data={barData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="name" />
                        <YAxis />
                        <Tooltip formatter={v => `${v?.toFixed(2)} ₴`} />
                        <Bar dataKey="value" fill="#0d6efd" />
                      </BarChart>
                    </ResponsiveContainer>
                  </>
                )}
              </div>
            </div>
            <div className="col-md-6">
              <div className="card border-0 shadow-sm p-3">
                <h6 className="text-muted mb-3">Топ категорій витрат</h6>
                {topCategories.length === 0
                  ? <div className="text-center text-muted py-4">Немає даних</div>
                  : <ResponsiveContainer width="100%" height={200}>
                    <PieChart>
                      <Pie data={topCategories} dataKey="totalAmount" nameKey="category" cx="50%" cy="50%" outerRadius={80} label={({ category }) => category}>
                        {topCategories.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
                      </Pie>
                      <Tooltip formatter={v => `${v?.toFixed(2)} ₴`} />
                    </PieChart>
                  </ResponsiveContainer>
                }
              </div>
            </div>
          </div>
        </>
      )}
      {activeTab === 'users' && (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-dark">
                <tr>
                  <th>Email</th>
                  <th>Роль</th>
                  <th>Статус</th>
                  <th>Останній вхід</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {users.length === 0
                  ? <tr><td colSpan={5} className="text-center text-muted py-4">Користувачів немає</td></tr>
                  : users.map(u => (
                    <tr key={u.userId || u.id}>
                      <td><span className="fw-semibold">{u.email}</span></td>
                      <td>
                        <select
                          className="form-select form-select-sm"
                          style={{ width: 120 }}
                          value={u.role}
                          onChange={e => handleRoleChange(u.userId || u.id, e.target.value)}
                        >
                          <option value="User">User</option>
                          <option value="Admin">Admin</option>
                        </select>
                      </td>
                      <td>
                        {u.isBlocked
                          ? <span className="badge bg-danger">Заблокований</span>
                          : <span className="badge bg-success">Активний</span>
                        }
                      </td>
                      <td className="text-muted small">
                        {u.lastLoginAt ? new Date(u.lastLoginAt).toLocaleString('uk-UA') : '—'}
                      </td>
                      <td>
                        <button
                          className={`btn btn-sm me-1 ${u.isBlocked ? 'btn-outline-success' : 'btn-outline-warning'}`}
                          onClick={() => handleBlock(u.userId || u.id, u.isBlocked)}
                        >
                          {u.isBlocked ? '🔓 Розблокувати' : '🔒 Блокувати'}
                        </button>
                        <button
                          className="btn btn-sm btn-outline-danger"
                          onClick={() => handleDelete(u.userId || u.id)}
                        >
                          🗑 Видалити
                        </button>
                      </td>
                    </tr>
                  ))
                }
              </tbody>
            </table>
          </div>
        </div>
      )}
    </Layout>
  )
}

export default AdminPage