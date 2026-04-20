namespace TextLivingDemo.Models;

/// <summary>
/// AI analysis results for SMS marketing campaign conversation
/// </summary>
public class AnalysisResponse
{
    /// <summary>
    /// Engagement score (0-100) - likelihood of customer taking action (click, purchase, review)
    /// </summary>
    public int EngagementScore { get; set; }

    /// <summary>
    /// Overall sentiment of customer (Positive, Neutral, Negative)
    /// </summary>
    public string Sentiment { get; set; } = string.Empty;

    /// <summary>
    /// Intent detected from customer responses (Interested, Browsing, Opted-Out, Needs-Support)
    /// </summary>
    public string CustomerIntent { get; set; } = string.Empty;

    /// <summary>
    /// Likelihood customer will click link (High, Medium, Low)
    /// </summary>
    public string LinkClickLikelihood { get; set; } = string.Empty;

    /// <summary>
    /// Array of 3 AI-generated optimized follow-up messages
    /// </summary>
    public List<string> NextBestMessages { get; set; } = new();

    /// <summary>
    /// Key insights from conversation analysis
    /// </summary>
    public List<string> Insights { get; set; } = new();

    /// <summary>
    /// A/B testing suggestions for improving campaign performance
    /// </summary>
    public List<string> AbTestSuggestions { get; set; } = new();

    /// <summary>
    /// Token usage for this API call (for cost tracking)
    /// </summary>
    public int TokensUsed { get; set; }

    /// <summary>
    /// Estimated cost in USD for this analysis
    /// </summary>
    public decimal EstimatedCost { get; set; }
}
