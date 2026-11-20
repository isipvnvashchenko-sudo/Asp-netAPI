import { useEffect, useState } from 'react'

const API = import.meta.env.VITE_API_URL || 'http://localhost:5203'

export default function App() {
  const [token, setToken] = useState(() => localStorage.getItem('token'))
  const [user, setUser] = useState(null)
  const [orders, setOrders] = useState(null)
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (token) loadMe()
  }, [token])

  async function loadMe() {
    const res = await fetch(`${API}/api/auth/me`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) {
      setUser(await res.json())
    } else {
      logout()
    }
  }

  async function handleLogin(e) {
    e.preventDefault()
    setError('')
    setBusy(true)
    try {
      const res = await fetch(`${API}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      })
      if (res.status === 429) {
        setError('Слишком много попыток входа. Подождите минуту.')
        return
      }
      if (!res.ok) {
        setError('Неверный логин или пароль.')
        return
      }
      const data = await res.json()
      localStorage.setItem('token', data.accessToken)
      setToken(data.accessToken)
    } catch {
      setError('Сервер недоступен. Запустите API на ' + API)
    } finally {
      setBusy(false)
    }
  }

  function logout() {
    localStorage.removeItem('token')
    setToken(null)
    setUser(null)
    setOrders(null)
  }

  async function loadOrders() {
    const res = await fetch(`${API}/api/orders`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) {
      setOrders(await res.json())
    } else {
      setOrders({ error: `HTTP ${res.status}` })
    }
  }

  if (!token) {
    return (
      <main className="card">
        <h1>SHOPDB</h1>
        <p className="hint">Демо JWT-аутентификации: React (порт 3000) → ASP.NET Core API (порт 5203)</p>
        <form onSubmit={handleLogin}>
          <label>
            Логин
            <input value={username} onChange={e => setUsername(e.target.value)} placeholder="admin" required />
          </label>
          <label>
            Пароль
            <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="••••••" required />
          </label>
          <button type="submit" disabled={busy}>{busy ? 'Входим…' : 'Войти'}</button>
          {error && <p className="error">{error}</p>}
        </form>
      </main>
    )
  }

  return (
    <main className="card">
      <div className="top">
        <h1>Профиль</h1>
        <button onClick={logout}>Выйти</button>
      </div>
      {user ? (
        <table>
          <tbody>
            <tr><th>Id</th><td>{user.id}</td></tr>
            <tr><th>Логин</th><td>{user.username}</td></tr>
            <tr><th>Email</th><td>{user.email ?? '—'}</td></tr>
            <tr><th>Роль</th><td><span className="role">{user.role?.name}</span></td></tr>
            <tr><th>Активен</th><td>{user.isActive ? 'да' : 'нет'}</td></tr>
          </tbody>
        </table>
      ) : (
        <p className="hint">Загрузка профиля…</p>
      )}
      <p className="hint">
        Данные выше получены запросом <code>GET {API}/api/auth/me</code> с заголовком
        <code>Authorization: Bearer …</code> — CORS-политика API разрешает origin http://localhost:3000
      </p>
      <button onClick={loadOrders}>Загрузить заказы (защищённый эндпоинт)</button>
      {orders && (
        orders.error ? <p className="error">Ошибка: {orders.error}</p> : (
          <ul>
            {orders.map(o => (
              <li key={o.id}>
                Заказ №{o.id} — {o.totalAmount} ₽, статус «{o.status}», позиций: {o.items?.length ?? 0}
              </li>
            ))}
          </ul>
        )
      )}
    </main>
  )
}
