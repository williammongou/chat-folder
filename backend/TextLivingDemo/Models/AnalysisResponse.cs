namespace TextLivingDemo.Models;

/// <summary>
/// AI analysis results for the conversation thread
/// </summary>
public class AnalysisResponse
{
    /// <summary>
    /// Conversion probability score (0-100)
    /// </summary>
    public int ConversionProbability { get; set; }

    /// <summary>
    /// Overall sentiment of prospect (Positive, Neutral, Negative)
    /// </summary>
    public string Sentiment { get; set; } = string.Empty;

    /// <summary>
    /// Urgency level of the prospect (Low, Medium, High)
    /// </summary>
    public string UrgencyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Array of 3 AI-generated recommended follow-up messages
    /// </summary>
    public List<string> NextBestMessages { get; set; } = new();

    /// <summary>
    /// Key insights from conversation analysis
    /// </summary>
    public List<string> Insights { get; set; } = new();

    /// <summary>
    /// Token usage for this API call (for cost tracking)
    /// </summary>
    public int TokensUsed { get; set; }

    /// <summary>
    /// Estimated cost in USD for this analysis
    /// </summary>
    public decimal EstimatedCost { get; set; }
}
