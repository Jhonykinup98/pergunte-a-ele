import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { getQuestions, createQuestion, answerQuestion } from '../services/api'

export default function Dashboard() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const isAdmin = user?.role === 'admin'

  const [questions, setQuestions] = useState([])
  const [newQuestion, setNewQuestion] = useState('')
  const [answerDrafts, setAnswerDrafts] = useState({})
  const [loading, setLoading] = useState(true)

  async function loadQuestions() {
    try {
      const { data } = await getQuestions()
      setQuestions(data)
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadQuestions()
  }, [])

  async function handleAsk(e) {
    e.preventDefault()
    if (!newQuestion.trim()) return
    await createQuestion(newQuestion)
    setNewQuestion('')
    loadQuestions()
  }

  async function handleAnswer(id) {
    const text = answerDrafts[id]
    if (!text?.trim()) return
    await answerQuestion(id, text)
    setAnswerDrafts({ ...answerDrafts, [id]: '' })
    loadQuestions()
  }

  function handleLogout() {
    logout()
    navigate('/login')
  }

  return (
    <div className="phone-stage top-stage">
      <div className="phone-frame chat-frame">
        <div className="chat-top">
          <span className="phone-eyebrow" style={{ margin: 0 }}>
            {isAdmin ? 'ELE' : `Olá, ${user?.login}`}
          </span>
            <button className="chat-logout" onClick={handleLogout}>Sair</button>
        </div>

        {!isAdmin && (
          <>
            <h1 className="ask-headline">
              Faça sua pergunta<br />e ELE irá responder
            </h1>
            <form className="ask-bar" onSubmit={handleAsk}>
              <input
                placeholder="Digite sua pergunta..."
                value={newQuestion}
                onChange={(e) => setNewQuestion(e.target.value)}
              />
              <button type="submit">Enviar</button>
            </form>
          </>
        )}

        {loading && <p className="answer-pending">Carregando...</p>}

        {!loading && questions.length === 0 && (
          <p className="answer-pending">
            {isAdmin ? 'Nenhuma pergunta ainda.' : 'Você ainda não fez nenhuma pergunta.'}
          </p>
        )}

        {questions.map((q) => (
          <div key={q.id} className="question-card">
            <p className="question-meta">
              {q.userLogin === user.login ? 'Você' : q.userLogin} · {new Date(q.createdAt).toLocaleDateString('pt-BR')}
            </p>
            <p className="question-content">{q.content}</p>

            {q.answerContent ? (
              <div className="answer-block">
                <b>ELE respondeu:</b> {q.answerContent}
              </div>
            ) : isAdmin ? (
              <div className="answer-form">
                <input
                  placeholder="Escreva a resposta..."
                  value={answerDrafts[q.id] || ''}
                  onChange={(e) =>
                    setAnswerDrafts({ ...answerDrafts, [q.id]: e.target.value })
                  }
                />
                <button onClick={() => handleAnswer(q.id)}>Responder</button>
              </div>
            ) : (
              <p className="answer-pending">Aguardando resposta...</p>
            )}
          </div>
        ))}
      </div>
    </div>
  )
}