import { useEffect, useState } from 'react'
import { getUsers, blockUser, unblockUser, deleteUser, changeUserRole } from '../api/admin'
import Layout from '../components/Layout'

const AdminPage = () => {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const fetchData = async () => {
    try {
      const { data } = await getUsers()
      setUsers(data || [])
    } catch (err) {
      setError('Помилка завантаження користувачів')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [])

  const handleBlock = async (id, isBlocked) => {
    try {
      if (isBlocked) {
        await unblockUser(id)
      } else {
        await blockUser(id)
      }
      fetchData()
    } catch {
      alert('Помилка зміни статусу')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('Видалити користувача? Це незворотня дія!')) return
    try {
      await deleteUser(id)
      fetchData()
    } catch {
      alert('Помилка видалення')
    }
  }

  const handleRoleChange = async (id, role) => {
    try {
      await changeUserRole(id, role)
      fetchData()
    } catch {
      alert('Помилка зміни ролі')
    }
  }

  if (loading) return <Layout><div className="text-center mt-5"><div className="spinner-border text-primary" /></div></Layout>

  return (
    <Layout>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">Адмін-панель</h4>
        <span className="badge bg-primary fs-6">{users.length} користувачів</span>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

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
                    <td>
                      <span className="fw-semibold">{u.email}</span>
                    </td>
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
                      {u.lastLoginAt
                        ? new Date(u.lastLoginAt).toLocaleString('uk-UA')
                        : '—'
                      }
                    </td>
                    <td>
                      <button
                        className={`btn btn-sm me-1 ${u.isBlocked ? 'btn-outline-success' : 'btn-outline-warning'}`}
                        onClick={() => handleBlock(u.userId || u.id, u.isBlocked)}
                      >
                        {u.isBlocked ? 'Розблокувати' : 'Блокувати'}
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
    </Layout>
  )
}

export default AdminPage