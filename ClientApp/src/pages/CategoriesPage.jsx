import { useEffect, useState } from 'react'
import { getCategories, createCategory, updateCategory, deleteCategory } from '../api/categories'
import Layout from '../components/Layout'

const CategoriesPage = () => {
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editing, setEditing] = useState(null)
  const [form, setForm] = useState({ name: '' })
  const [error, setError] = useState('')

  const fetchData = async () => {
    try {
      const { data } = await getCategories()
      setCategories(data || [])
    } catch {}
    finally { setLoading(false) }
  }

  useEffect(() => { fetchData() }, [])

  const openCreate = () => {
    setEditing(null)
    setForm({ name: '' })
    setError('')
    setShowModal(true)
  }

  const openEdit = (cat) => {
    setEditing(cat)
    setForm({ name: cat.name || '' })
    setError('')
    setShowModal(true)
  }

  const handleSubmit = async () => {
    if (!form.name) { setError('Введіть назву категорії'); return }
    try {
      if (editing) {
        await updateCategory(editing.categoryId || editing.id, form)
      } else {
        await createCategory(form)
      }
      setShowModal(false)
      fetchData()
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка збереження')
    }
  }

  const handleDelete = async (id) => {
    if (!confirm('Видалити категорію?')) return
    await deleteCategory(id)
    fetchData()
  }

  if (loading) return <Layout><div className="text-center mt-5"><div className="spinner-border text-primary" /></div></Layout>

  return (
    <Layout>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">Категорії</h4>
        <button className="btn btn-primary" onClick={openCreate}>+ Додати</button>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="table-responsive">
          <table className="table table-hover mb-0">
            <thead className="table-light">
              <tr>
                <th>#</th>
                <th>Назва</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {categories.length === 0
                ? <tr><td colSpan={3} className="text-center text-muted py-4">Категорій немає</td></tr>
                : categories.map((c, i) => (
                  <tr key={c.categoryId || c.id}>
                    <td className="text-muted">{i + 1}</td>
                    <td>{c.name}</td>
                    <td>
                      <button className="btn btn-sm btn-outline-secondary me-1" onClick={() => openEdit(c)}>✏️</button>
                      <button className="btn btn-sm btn-outline-danger" onClick={() => handleDelete(c.categoryId || c.id)}>🗑</button>
                    </td>
                  </tr>
                ))
              }
            </tbody>
          </table>
        </div>
      </div>

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{editing ? 'Редагувати категорію' : 'Нова категорія'}</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                {error && <div className="alert alert-danger">{error}</div>}
                <div className="mb-3">
                  <label className="form-label">Назва</label>
                  <input
                    type="text"
                    className="form-control"
                    value={form.name}
                    onChange={e => setForm({ name: e.target.value })}
                    placeholder="Наприклад: Продукти"
                  />
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

export default CategoriesPage