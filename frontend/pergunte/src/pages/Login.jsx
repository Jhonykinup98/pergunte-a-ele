import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import FormInput from '../components/FormInput'
import GoogleLoginButton from '../components/GoogleLoginButton'
import { loginUser } from '../services/api'
import { useAuth } from '../context/AuthContext'

export default function Login() {
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const { login: setAuth } = useAuth()
  const navigate = useNavigate()

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    try {
      const { data } = await loginUser({ login, password })
      setAuth(data)
      navigate('/dashboard')
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao entrar.')
    }
  }

  return (
    <div className="phone-stage login-stage">
      <div className="phone-frame">
        <div className="phone-notch" />

        <h1 className="phone-title">ELE</h1>

        <form onSubmit={handleSubmit}>
          <FormInput label="Login" value={login} onChange={(e) => setLogin(e.target.value)} />
          <FormInput
            label="Senha"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          {error && <p className="error-text">{error}</p>}

          <button type="submit" className="btn-primary">
            Entrar
          </button>
        </form>

        <div className="divider">ou</div>

        <div className="google-slot">
          <GoogleLoginButton />
        </div>

        <p className="footer-text">
          Não tem conta? <Link to="/cadastro">Cadastre-se</Link>
        </p>
      </div>
    </div>
  )
}