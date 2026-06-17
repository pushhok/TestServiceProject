import { getToken } from './auth'

const API_BASE = '/api'

function getHeaders() {
  const headers = { 'Content-Type': 'application/json' }
  const token = getToken()
  if (token) headers['Authorization'] = `Bearer ${token}`
  return headers
}

export async function getTests() {
  const response = await fetch(`${API_BASE}/tests`)
  if (!response.ok) throw new Error('Не удалось загрузить тесты')
  return response.json()
}

export async function getTestById(id) {
  const response = await fetch(`${API_BASE}/tests/${id}`)
  if (!response.ok) throw new Error('Не удалось загрузить тест')
  return response.json()
}

export async function createTest(testData) {
  const response = await fetch(`${API_BASE}/tests`, {
    method: 'POST',
    headers: getHeaders(),
    body: JSON.stringify(testData)
  })
  const data = await response.json()
  if (!response.ok) throw new Error(data.message || 'Не удалось создать тест')
  return data
}

export async function updateTest(id, testData) {
  const response = await fetch(`${API_BASE}/tests/${id}`, {
    method: 'PUT',
    headers: getHeaders(),
    body: JSON.stringify(testData)
  })
  const data = await response.json()
  if (!response.ok) throw new Error(data.message || 'Не удалось обновить тест')
  return data
}

export async function deleteTest(id) {
  const response = await fetch(`${API_BASE}/tests/${id}`, {
    method: 'DELETE',
    headers: getHeaders()
  })
  const data = await response.json()
  if (!response.ok) throw new Error(data.message || 'Не удалось удалить тест')
  return data
}

export async function submitTest(testId, answers) {
  const response = await fetch(`${API_BASE}/tests/${testId}/submit`, {
    method: 'POST',
    headers: getHeaders(),
    body: JSON.stringify({ answers })
  })
  const data = await response.json()
  if (!response.ok) throw new Error(data.message || 'Не удалось отправить тест')
  return data
}

export async function getResults() {
  const response = await fetch(`${API_BASE}/results`, {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Не удалось загрузить результаты')
  return response.json()
}

export async function getMyTests() {
  const response = await fetch(`${API_BASE}/my-tests`, {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Не удалось загрузить мои тесты')
  return response.json()
}