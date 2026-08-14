import { GoogleLogin } from '@react-oauth/google'
import { loginWithGoogle } from '../services/api'
import { useAuth } from '../context/AuthContext'
import { useNavigate } from 'react-router-dom'

export default function GoogleLoginButton() {
  const { login } = useAuth()
  const navigate = useNavigate()

  async function handleSuccess(credentialResponse) {
    try {
      const { data } = await loginWithGoogle(credentialResponse.credential)
      login(data)
      navigate('/dashboard')
    } catch (err) {
      alert('Não foi possível entrar com Google. Tente novamente.')
    }
  }

  return (
    <GoogleLogin
      onSuccess={handleSuccess}
      onError={() => alert('Falha no login com Google.')}
    />
  )
}
