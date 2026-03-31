import { useEffect, useState } from 'react'
import { getBudgetSummary, createBudget } from '../api/budgets'
import Layout from '../components/Layout'

const MONTHS = [
  'Січень','Лютий','Березень','Квітень','Травень','Червень',
  'Липень','Серпень','Вересень','Жовтень','Листопад','Грудень'
]

const BudgetsPage = () => {
  const now = new Date()
  const [month, setMonth] = useState(now.getMonth() + 1)
  const [year, setYear] = useState(now.getFullYear())
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState({ monthlyLimit: '' })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  const fetchData = async () => {
    setLoading(true)
    try {
      const { data } = await getBudgetSummary(month, year)
      setSummary(data)
    } catch {
      setSummary(null)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [month, year])

  const handleSubmit = async () => {
    if (!form.monthlyLimit) { setError('Введіть ліміт'); return }
    setSaving(true)
    try {
      await createBudget({
        month,
        year,
        monthlyLimit: parseFloat(form.monthlyLimit)
      })
      setShowModal(false)
      fetchData()
    } catch (err) {
      setError(err.response?.data?.message || 'Помилка збереження')
    } finally {
      setSaving(false)
    }
  }

  const pct = summary ? Math.min(Math.round(summary.usagePercentage), 100) : 0
  const progressColor = pct >= 90 ? 'bg-danger' : pct >= 70 ? 'bg-warning' : 'bg-success'

  return (
    <Layout>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="mb-0">Бюджет</h4>
        <button className="btn btn-primary" onClick={() => { setForm({ monthlyLimit: '' }); setError(''); setShowModal(true) }}>
          Встановити ліміт
        </button>
      </div>

      <div className="card border-0 shadow-sm p-3 mb-4">
        <div className="row g-2 align-items-center">
          <div className="col-auto">
            <select className="form-select" value={month} onChange={e => setMonth(+e.target.value)}>
              {MONTHS.map((m, i) => <option key={i+1} value={i+1}>{m}</option>)}
            </select>
          </div>
          <div className="col-auto">
            <select className="form-select" value={year} onChange={e => setYear(+e.target.value)}>
              {[2024, 2025, 2026, 2027].map(y => <option key={y} value={y}>{y}</option>)}
            </select>
          </div>
        </div>
      </div>

      {loading
        ? <div className="text-center mt-5"><div className="spinner-border text-primary" /></div>
        : summary && summary.limit > 0
          ? <div className="row g-3">
              <div className="col-md-4">
                <div className="card border-0 shadow-sm text-center p-3">
                  <div className="text-muted small mb-1">Ліміт</div>
                  <h3 className="fw-bold text-primary">{summary.limit?.toFixed(2)} ₴</h3>
                </div>
              </div>
              <div className="col-md-4">
                <div className="card border-0 shadow-sm text-center p-3">
                  <div className="text-muted small mb-1">Витрачено</div>
                  <h3 className="fw-bold text-danger">{summary.used?.toFixed(2)} ₴</h3>
                </div>
              </div>
              <div className="col-md-4">
                <div className="card border-0 shadow-sm text-center p-3">
                  <div className="text-muted small mb-1">Залишок</div>
                  <h3 className={`fw-bold ${summary.remaining >= 0 ? 'text-success' : 'text-danger'}`}>
                    {summary.remaining?.toFixed(2)} ₴
                  </h3>
                </div>
              </div>
              <div className="col-12">
                <div className="card border-0 shadow-sm p-4">
                  <div className="d-flex justify-content-between mb-2">
                    <span>Використано бюджету</span>
                    <span className="fw-bold">{pct}%</span>
                  </div>
                  <div className="progress" style={{ height: 20 }}>
                    <div className={`progress-bar ${progressColor}`} style={{ width: `${pct}%` }} />
                  </div>
                  {summary.exceeded && (
                    <div className="alert alert-danger mt-3 mb-0">
                      Бюджет перевищено!
                    </div>
                  )}
                </div>
              </div>
            </div>
          : <div className="text-center text-muted mt-5">
              <p>Бюджет на {MONTHS[month-1]} {year} не встановлено</p>
              <button className="btn btn-primary" onClick={() => { setForm({ monthlyLimit: '' }); setError(''); setShowModal(true) }}>
                Встановити ліміт
              </button>
            </div>
      }

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Ліміт на {MONTHS[month-1]} {year}</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                {error && <div className="alert alert-danger">{error}</div>}
                <div className="mb-3">
                  <label className="form-label">Місячний ліміт (₴)</label>
                  <input
                    type="number"
                    className="form-control"
                    value={form.monthlyLimit}
                    onChange={e => setForm({ monthlyLimit: e.target.value })}
                    placeholder="5000"
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Скасувати</button>
                <button className="btn btn-primary" onClick={handleSubmit} disabled={saving}>
                  {saving ? 'Зберігаємо...' : 'Зберегти'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </Layout>
  )
}

export default BudgetsPage