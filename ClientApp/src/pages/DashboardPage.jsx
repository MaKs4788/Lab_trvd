import { useEffect, useState } from 'react'
import { getExpenses } from '../api/expenses'
import { getIncomes } from '../api/incomes'
import { useAuth } from '../context/AuthContext'
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid, Legend } from 'recharts'
import Layout from '../components/Layout'

const COLORS = ['#dc3545', '#198754']

const DashboardPage = () => {
  const { user } = useAuth()
  const [expenses, setExpenses] = useState([])
  const [incomes, setIncomes] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchAll = async () => {
      try {
        const [e, i] = await Promise.all([getExpenses(), getIncomes()])
        setExpenses(e.data || [])
        setIncomes(i.data || [])
      } catch (err) {
        console.error(err)
      } finally {
        setLoading(false)
      }
    }
    fetchAll()
  }, [])

  const totalExpenses = expenses.reduce((s, e) => s + (e.amount || 0), 0)
  const totalIncomes = incomes.reduce((s, i) => s + (i.amount || 0), 0)
  const balance = totalIncomes - totalExpenses

  const now = new Date()
  const currentMonth = now.getMonth()
  const currentYear = now.getFullYear()

  const monthExpenses = expenses
    .filter(e => new Date(e.date).getMonth() === currentMonth && new Date(e.date).getFullYear() === currentYear)
    .reduce((s, e) => s + (e.amount || 0), 0)

  const monthIncomes = incomes
    .filter(i => new Date(i.date).getMonth() === currentMonth && new Date(i.date).getFullYear() === currentYear)
    .reduce((s, i) => s + (i.amount || 0), 0)

  const pieData = [
    { name: 'Витрати', value: totalExpenses },
    { name: 'Доходи', value: totalIncomes }
  ]

  const barData = [
    { name: 'Цей місяць', Доходи: monthIncomes, Витрати: monthExpenses },
    { name: 'Всього', Доходи: totalIncomes, Витрати: totalExpenses }
  ]

  if (loading) return <Layout><div className="text-center mt-5"><div className="spinner-border text-primary" /></div></Layout>

  return (
    <Layout>
      <h4 className="mb-4">Привіт, <strong>{user?.email}</strong> 👋</h4>

      <div className="row g-3 mb-4">
        <div className="col-md-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="text-muted small mb-1">Загальний баланс</div>
              <h3 className={`fw-bold ${balance >= 0 ? 'text-success' : 'text-danger'}`}>
                {balance.toFixed(2)} ₴
              </h3>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="text-muted small mb-1">Всього доходів</div>
              <h3 className="fw-bold text-success">{totalIncomes.toFixed(2)} ₴</h3>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="text-muted small mb-1">Всього витрат</div>
              <h3 className="fw-bold text-danger">{totalExpenses.toFixed(2)} ₴</h3>
            </div>
          </div>
        </div>
        <div className="col-md-6">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="text-muted small mb-1">Доходи цього місяця</div>
              <h4 className="fw-bold text-success">{monthIncomes.toFixed(2)} ₴</h4>
            </div>
          </div>
        </div>
        <div className="col-md-6">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="text-muted small mb-1">Витрати цього місяця</div>
              <h4 className="fw-bold text-danger">{monthExpenses.toFixed(2)} ₴</h4>
            </div>
          </div>
        </div>
      </div>

      <div className="row g-3">
        <div className="col-md-5">
          <div className="card border-0 shadow-sm p-3">
            <h6 className="mb-3 text-muted">Доходи vs Витрати (всього)</h6>
            <ResponsiveContainer width="100%" height={250}>
              <PieChart>
                <Pie data={pieData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={90} label>
                  {pieData.map((_, i) => <Cell key={i} fill={COLORS[i]} />)}
                </Pie>
                <Tooltip formatter={(v) => `${v.toFixed(2)} ₴`} />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
        <div className="col-md-7">
          <div className="card border-0 shadow-sm p-3">
            <h6 className="mb-3 text-muted">Порівняння</h6>
            <ResponsiveContainer width="100%" height={250}>
              <BarChart data={barData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip formatter={(v) => `${v.toFixed(2)} ₴`} />
                <Legend />
                <Bar dataKey="Доходи" fill="#198754" />
                <Bar dataKey="Витрати" fill="#dc3545" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </Layout>
  )
}

export default DashboardPage