export interface Message {
  sender: string;
  text: string;
  timestamp: Date;
}

export interface ConversationRequest {
  messages: Message[];
}

export interface AnalysisResponse {
  conversionProbability: number;
  sentiment: string;
  urgencyLevel: string;
  nextBestMessages: string[];
  insights: string[];
  tokensUsed: number;
  estimatedCost: number;
}
