import { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { confirmEmail } from '../services/api'

export default function ConfirmEmail() {
  const [searchParams] = useSearchParams()
  const [status, setStatus] = useState('carregando') 
  const navigate = useNavigate()

  useEffect(() => {
    const token = searchParams.get('token')
    if (!token) {
      setStatus('erro')
      return
    }

    confirmEmail(token)
      .then(() => setStatus('sucesso'))
      .catch(() => setStatus('erro'))
  }, [searchParams])

  return (
    <div style={{ maxWidth: 360, margin: '4rem auto', textAlign: 'center' }}>
      {status === 'carregando' && <p>Confirmando seu e-mail...</p>}

      {status === 'sucesso' && (
        <>
          <h2>E-mail confirmado! ✅</h2>
          <button onClick={() => navigate('/login')}>Ir para o login</button>
        </>
      )}

      {status === 'erro' && (
        <>
          <h2>Link inválido ou expirado</h2>
          <button onClick={() => navigate('/cadastro')}>Voltar ao cadastro</button>
        </>
      )}
    </div>
  )
}
