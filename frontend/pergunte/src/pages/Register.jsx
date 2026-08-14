import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import FormInput from '../components/FormInput'
import { registerUser } from '../services/api'

export default function Register() {
  const [form, setForm] = useState({
    login: '',
    email: '',
    password: '',
    confirmPassword: ''
  })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  const navigate = useNavigate()

  function update(field) {
    return (e) => setForm({ ...form, [field]: e.target.value })
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')

    if (form.password !== form.confirmPassword) {
      setError('As senhas não coincidem.')
      return
    }

    try {
      await registerUser(form)
      setSuccess(true)
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao cadastrar.')
    }
  }

  if (success) {
    return (
          <div className="phone-stage top-stage">
          <div className="phone-frame">
          <div className="phone-notch" />
          <p className="phone-eyebrow">Quase lá</p>
          <h1 className="phone-title" style={{ fontSize: 'clamp(1.8rem, 5vw, 2.3rem)', color: 'var(--accent)' }}>
            Confirme seu e-mail
          </h1>
          <p style={{ textAlign: 'center', color: 'var(--ink-muted)', marginBottom: '2rem' }}>
            Enviamos um link de confirmação para <b>{form.email}</b>.
          </p>
          <button className="btn-primary" onClick={() => navigate('/login')}>
            Voltar ao login
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="phone-stage top-stage">
      <div className="phone-frame">
        <div className="phone-notch" />

        <h1 className="phone-title" style={{ fontSize: 'clamp(1.8rem, 5vw, 2.3rem)', color: 'var(--accent)' }}>
          CRIAR CONTA
        </h1>

        <form onSubmit={handleSubmit}>
          <FormInput label="Login" value={form.login} onChange={update('login')} />
          <FormInput label="Gmail" type="email" value={form.email} onChange={update('email')} />
          <FormInput
            label="Senha"
            type="password"
            value={form.password}
            onChange={update('password')}
          />
          <FormInput
            label="Repetir senha"
            type="password"
            value={form.confirmPassword}
            onChange={update('confirmPassword')}
          />

          {error && <p className="error-text">{error}</p>}

          <button type="submit" className="btn-primary">
            Cadastrar
          </button>
        </form>

        <p className="footer-text">
          Já tem conta? <Link to="/login">Entrar</Link>
        </p>
      </div>
    </div>
  )
}