export interface HistoricalCampaignData {
  businessName: string;
  businessType: string;
  campaigns: PastCampaign[];
  insights?: CampaignInsights;
}

export interface PastCampaign {
  id: string;
  outgoingText: string;
  sentAt: string;
  recipientCount: number;
  incomingReplies: IncomingReply[];
  footTraffic: number;
  conversions: number;
  conversionRate: number;
  unsubscribeRate: number;
  notes: string;
}

export interface IncomingReply {
  text: string;
  count: number;
}

export interface CampaignInsights {
  bestPerformingCampaign: string;
  worstPerformingCampaign: string;
  averageConversionRate: number;
  averageUnsubscribeRate: number;
  bestSendTime: string;
  redFlags: string[];
  successPatterns: string[];
}

export interface CampaignGenerationRequest {
  prompt: string;
  historicalData?: HistoricalCampaignData;
}

export interface CampaignGenerationResponse {
  option1: CampaignOption;
  option2: CampaignOption;
  tokensUsed: number;
  totalTokensToday: number;
  estimatedCost: number;
}

export interface CampaignOption {
  messageText: string;
  whyThisWorks: string;
  bestSendTime: string;
  insights: string[];
  predictedConversionRate: number;
  toneAnalysis: string;
}

export interface TokenUsageResponse {
  tokensUsedToday: number;
  tokenLimit: number;
  percentageUsed: number;
  status: string; // "green" | "orange" | "red"
}

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
  campaignResult?: CampaignGenerationResponse;
  timestamp: Date;
}
