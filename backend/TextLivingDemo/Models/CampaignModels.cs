namespace TextLivingDemo.Models;

/// <summary>
/// Historical campaign data uploaded by user
/// </summary>
public class HistoricalCampaignData
{
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public List<PastCampaign> Campaigns { get; set; } = new();
    public CampaignInsights? Insights { get; set; }
}

/// <summary>
/// Individual past campaign with performance metrics
/// </summary>
public class PastCampaign
{
    public string Id { get; set; } = string.Empty;
    public string OutgoingText { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public int RecipientCount { get; set; }
    public List<IncomingReply> IncomingReplies { get; set; } = new();
    public int FootTraffic { get; set; }
    public int Conversions { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal UnsubscribeRate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Incoming customer replies to a campaign
/// </summary>
public class IncomingReply
{
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Insights from historical data
/// </summary>
public class CampaignInsights
{
    public string BestPerformingCampaign { get; set; } = string.Empty;
    public string WorstPerformingCampaign { get; set; } = string.Empty;
    public decimal AverageConversionRate { get; set; }
    public decimal AverageUnsubscribeRate { get; set; }
    public string BestSendTime { get; set; } = string.Empty;
    public List<string> RedFlags { get; set; } = new();
    public List<string> SuccessPatterns { get; set; } = new();
}

/// <summary>
/// Request to generate new campaign
/// </summary>
public class CampaignGenerationRequest
{
    public string Prompt { get; set; } = string.Empty;
    public HistoricalCampaignData? HistoricalData { get; set; }
}

/// <summary>
/// Response with 2 campaign options
/// </summary>
public class CampaignGenerationResponse
{
    public CampaignOption Option1 { get; set; } = new();
    public CampaignOption Option2 { get; set; } = new();
    public int TokensUsed { get; set; }
    public int TotalTokensToday { get; set; }
    public decimal EstimatedCost { get; set; }
}

/// <summary>
/// Individual campaign option
/// </summary>
public class CampaignOption
{
    public string MessageText { get; set; } = string.Empty;
    public string WhyThisWorks { get; set; } = string.Empty;
    public string BestSendTime { get; set; } = string.Empty;
    public List<string> Insights { get; set; } = new();
    public decimal PredictedConversionRate { get; set; }
    public string ToneAnalysis { get; set; } = string.Empty;
}

/// <summary>
/// Token usage tracking
/// </summary>
public class TokenUsageResponse
{
    public int TokensUsedToday { get; set; }
    public int TokenLimit { get; set; } = 10000;
    public decimal PercentageUsed { get; set; }
    public string Status { get; set; } = "green"; // green, orange, red
}
