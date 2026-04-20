export interface Message {
  sender: string;
  text: string;
  timestamp: Date;
}

export interface ConversationRequest {
  messages: Message[];
}

export interface AnalysisResponse {
  engagementScore: number;
  sentiment: string;
  customerIntent: string;
  linkClickLikelihood: string;
  nextBestMessages: string[];
  insights: string[];
  abTestSuggestions: string[];
  tokensUsed: number;
  estimatedCost: number;
}
