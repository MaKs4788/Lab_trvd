import { useEffect, useState } from 'react'
import { getExpenses, createExpense, updateExpense, deleteExpense } from '../api/expenses'
import { getCategories } from '../api/categories'
import Layout from '../components/Layout'

const ExpensesPage = () => {
  const [expenses, setExpenses] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editing, setEditing] = useState(null)
  const [form, setForm] = useState({ amount: '', description: '', date: '', categoryId: '' })
  const [error, setError] = useState('')

  const fetchData = async () => {
    try {
      const [e, c] = await Promise.all([getExpenses(), getCategories()])
      setExpenses(e.data || [])
      setCategories(c.data || [])
    } catch {}
    finally { setLoading(false) }
  }

  useEffect(() => { fetchData() }, [])

  const openCreate = () => {
    setEditing(null)
    setForm({ amount: '', description: '', date: new Date().toISOString().split('T')[0], categoryId: '' })
    setError('')
    setShowModal(true)
  }

  const openEdit = (expense) => {
    setEditing(expense)
    setForm({
      amount: expense.amount,
      description: expense.description || '',
      date: expense.date?.split('T')[0] || '',
      categoryId: expense.categoryId || ''
    })
    setError('')
    setShowModal(true)
  }

  const handleSubmit = async () => {
  if (!form.amount || !form.date) { setError('Заповніть суму та дату'); return }
  try {
    const payload = {
      amount: parseFloat(form.amount),
      description: form.description || null,
      date: form.date,
      categoryId: form.categoryId ? parseInt(form.categoryId) : null
    }
    if (editing) {
      await updateExpense(editing.expenseId || editing.id, payload)
    } else {
      await createExpense(payload)
    }
    setShowModal(false)
    fetchData()
  } catch (err) {
    setError(err.response?.data?.message || 'Помилка збереження')
  }
}

  const handleDelete = async (id) => {
    if (!confirm('Видалити витрату?')) return
    await deleteExpense(id)
    fetchData()
  }

  if (loading) return <Layout><div className="text-center mt-5"><div className="spinner-border text-primary" /></div></Layout>

  return (
    <Layout>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">💸 Витрати</h4>
        <button className="btn btn-primary" onClick={openCreate}>+ Додати</button>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="table-responsive">
          <table className="table table-hover mb-0">
            <thead className="table-light">
              <tr>
                <th>Дата</th>
                <th>Опис</th>
                <th>Категорія</th>
                <th>Сума</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {expenses.length === 0
                ? <tr><td colSpan={5} className="text-center text-muted py-4">Витрат немає</td></tr>
                : expenses.map(e => (
                  <tr key={e.expenseId || e.id}>
                    <td>{new Date(e.date).toLocaleDateString('uk-UA')}</td>
                    <td>{e.description || '—'}</td>
                    <td>
  <span className="badge bg-secondary">
    {categories.find(c => (c.categoryId || c.id) === e.categoryId)?.name || '—'}
  </span>
</td>
                    <td className="text-danger fw-semibold">-{e.amount?.toFixed(2)} ₴</td>
                    <td>
                      <button className="btn btn-sm btn-outline-secondary me-1" onClick={() => openEdit(e)}>✏️</button>
                      <button className="btn btn-sm btn-outline-danger" onClick={() => handleDelete(e.expenseId || e.id)}>🗑</button>
                    </td>
                  </tr>
                ))
              }
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal */}
      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{editing ? 'Редагувати витрату' : 'Нова витрата'}</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                {error && <div className="alert alert-danger">{error}</div>}
                <div className="mb-3">
                  <label className="form-label">Сума (₴)</label>
                  <input type="number" className="form-control" value={form.amount}
                    onChange={e => setForm({ ...form, amount: e.target.value })} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Опис</label>
                  <input type="text" className="form-control" value={form.description}
                    onChange={e => setForm({ ...form, description: e.target.value })} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Дата</label>
                  <input type="date" className="form-control" value={form.date}
                    onChange={e => setForm({ ...form, date: e.target.value })} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Категорія</label>
                  <select className="form-select" value={form.categoryId}
                    onChange={e => setForm({ ...form, categoryId: e.target.value })}>
                    <option value="">— Без категорії —</option>
                    {categories.map(c => (
                      <option key={c.categoryId || c.id} value={c.categoryId || c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Скасувати</button>
                <button className="btn btn-primary" onClick={handleSubmit}>Зберегти</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </Layout>
  )
}

export default ExpensesPage