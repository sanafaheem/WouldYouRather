import type { Question, VoteRequest, VoteResponse } from '../types/question'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string

async function parseJsonOrThrow<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const body = await response.text()
    throw new Error(`Request failed (${response.status}): ${body}`)
  }
  return (await response.json()) as T
}

export function getTodayQuestion(): Promise<Question> {
  return fetch(`${API_BASE_URL}/api/question/today`).then((res) =>
    parseJsonOrThrow<Question>(res),
  )
}

export function submitVote(request: VoteRequest): Promise<VoteResponse> {
  return fetch(`${API_BASE_URL}/api/question/vote`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  }).then((res) => parseJsonOrThrow<VoteResponse>(res))
}
