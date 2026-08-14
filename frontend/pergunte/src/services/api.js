import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

// Anexa o token JWT (se existir) em toda requisição
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const registerUser = (data) => api.post('/auth/register', data)
export const loginUser = (data) => api.post('/auth/login', data)
export const confirmEmail = (token) => api.get(`/auth/confirm-email?token=${token}`)
export const loginWithGoogle = (idToken) => api.post('/auth/google', { idToken })
export const getQuestions = () => api.get('/questions')
export const createQuestion = (content) => api.post('/questions', { content })
export const answerQuestion = (id, answerContent) => api.post(`/questions/${id}/answer`, { answerContent })

export default api
