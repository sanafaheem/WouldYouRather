export interface Question {
  id: string
  questionDescription: string
  optionA: string
  optionB: string
}

export type VoteOption = 'A' | 'B'

export interface VoteRequest {
  questionId: string
  option: VoteOption
}

export interface VoteResponse {
  votesA: number
  votesB: number
  percentA: number
  percentB: number
  wittyComment: string
}
