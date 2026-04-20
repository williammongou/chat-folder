using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using System.Text.Json;
using TextLivingDemo.Models;

namespace TextLivingDemo.Services;

/// <summary>
/// Service for analyzing apartment leasing conversations using Claude AI
/// </summary>
public class ConversationAnalysisService
{
    private readonly AnthropicClient _anthropicClient;
    private readonly ILogger<ConversationAnalysisService> _logger;

    // Claude Sonnet 4.5 pricing (as of 2025)
    private const decimal INPUT_COST_PER_MILLION = 3.00m;
    private const decimal OUTPUT_COST_PER_MILLION = 15.00m;

    public ConversationAnalysisService(IConfiguration configuration, ILogger<ConversationAnalysisService> logger)
    {
        var apiKey = configuration["Claude:ApiKey"]
            ?? throw new InvalidOperationException("Claude API key not configured");

        _anthropicClient = new AnthropicClient(apiKey);
        _logger = logger;
    }

    /// <summary>
    /// Analyzes a conversation thread and returns AI-powered insights
    /// </summary>
    public async Task<AnalysisResponse> AnalyzeConversationAsync(List<Models.Message> messages)
    {
        try
        {
            _logger.LogInformation("Analyzing conversation with {MessageCount} messages", messages.Count);

            // Build the conversation context for Claude
            var conversationText = BuildConversationText(messages);

            // Create the analysis prompt
            var prompt = BuildAnalysisPrompt(conversationText);

            // Call Claude API using the correct SDK structure
            var parameters = new MessageParameters
            {
                Messages = new List<Anthropic.SDK.Messaging.Message>
                {
                    new Anthropic.SDK.Messaging.Message(RoleType.User, prompt)
                },
                MaxTokens = 2000,
                Model = "claude-sonnet-4-20250514",
                Stream = false,
                Temperature = 0.7m
            };

            var response = await _anthropicClient.Messages.GetClaudeMessageAsync(parameters);

            // Parse the response
            var analysisResult = ParseClaudeResponse(response);

            // Calculate costs
            var inputTokens = response.Usage.InputTokens;
            var outputTokens = response.Usage.OutputTokens;
            var totalTokens = inputTokens + outputTokens;
            var estimatedCost = CalculateCost(inputTokens, outputTokens);

            analysisResult.TokensUsed = totalTokens;
            analysisResult.EstimatedCost = estimatedCost;

            _logger.LogInformation("Analysis completed. Tokens: {Tokens}, Cost: ${Cost:F4}",
                totalTokens, estimatedCost);

            return analysisResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing conversation");
            throw;
        }
    }

    /// <summary>
    /// Builds a formatted conversation text from messages
    /// </summary>
    private string BuildConversationText(List<Models.Message> messages)
    {
        var lines = messages
            .OrderBy(m => m.Timestamp)
            .Select(m => $"[{m.Timestamp:yyyy-MM-dd HH:mm}] {m.Sender}: {m.Text}");

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Creates the Claude prompt for SMS marketing campaign analysis
    /// This is optimized for structured JSON output
    /// </summary>
    private string BuildAnalysisPrompt(string conversationText)
    {
        return $@"You are an expert SMS marketing analyst for TextLiving, analyzing customer responses to promotional text campaigns. Your goal is to help businesses optimize their SMS marketing for maximum engagement, link clicks, purchases, and reviews.

SMS CONVERSATION THREAD:
{conversationText}

Analyze this SMS conversation and provide:

1. ENGAGEMENT SCORE (0-100): Overall likelihood customer will take desired action (click link, make purchase, leave review)
   - Consider: response rate, message tone, question depth, expressed interest, buying signals
   - High score (70-100): Enthusiastic, asking questions, clicked links, ready to purchase, agreed to leave review
   - Medium score (40-69): Polite interest, some engagement, considering offer, might click link
   - Low score (0-39): Minimal response, disinterest, opt-out requests, negative feedback

2. SENTIMENT: Overall emotional tone of customer's messages (Positive, Neutral, Negative)
   - Positive: Enthusiastic, grateful, excited about offer
   - Neutral: Polite, informational, transactional
   - Negative: Frustrated, annoyed, requesting opt-out

3. CUSTOMER INTENT: What the customer is trying to do (Interested, Browsing, Opted-Out, Needs-Support)
   - Interested: Wants to learn more, clicked link, asking about products/services
   - Browsing: Considering but not committed, might buy later
   - Opted-Out: Wants to unsubscribe, getting too many messages
   - Needs-Support: Has questions, issues, or needs help

4. LINK CLICK LIKELIHOOD (High, Medium, Low): How likely customer will click promotional links
   - High: Has clicked before, asking for links, expressing strong interest
   - Medium: Showing some interest, might click with right offer
   - Low: Not interested, has opted out, or ignores messages

5. NEXT BEST MESSAGES: Generate exactly 3 optimized follow-up SMS messages to maximize engagement. These should:
   - Be concise (SMS-appropriate, under 160 characters when possible)
   - Include clear call-to-action with link or offer
   - Be personalized to customer's interests and behavior
   - Use proven SMS marketing tactics (urgency, exclusivity, value)
   - For review requests: make it easy with direct link and incentive
   - Each message should drive specific action (click, purchase, review)

6. INSIGHTS: Provide 3-5 key observations about:
   - Customer's buying intent and readiness to purchase
   - What offers or products they're most interested in
   - Optimal timing for follow-up messages
   - Red flags (opt-out risk, fatigue, negative sentiment)
   - Opportunities to increase engagement

7. A/B TEST SUGGESTIONS: Provide 2-3 specific A/B testing ideas to improve campaign performance:
   - Different message timing
   - Offer variations (discount %, free shipping, BOGO, etc.)
   - CTA variations (Shop Now, Learn More, Get Deal, etc.)
   - Personalization approaches
   - Link placement and formatting

Return your analysis as valid JSON matching this exact structure:
{{
  ""engagementScore"": <number 0-100>,
  ""sentiment"": ""<Positive|Neutral|Negative>"",
  ""customerIntent"": ""<Interested|Browsing|Opted-Out|Needs-Support>"",
  ""linkClickLikelihood"": ""<High|Medium|Low>"",
  ""nextBestMessages"": [
    ""<SMS message 1>"",
    ""<SMS message 2>"",
    ""<SMS message 3>""
  ],
  ""insights"": [
    ""<insight 1>"",
    ""<insight 2>"",
    ""<insight 3>""
  ],
  ""abTestSuggestions"": [
    ""<test idea 1>"",
    ""<test idea 2>"",
    ""<test idea 3>""
  ]
}}

IMPORTANT: Return ONLY the JSON object, no additional text or formatting.";
    }

    /// <summary>
    /// Parses Claude's response into structured analysis
    /// </summary>
    private AnalysisResponse ParseClaudeResponse(MessageResponse response)
    {
        var textContent = response.Content.FirstOrDefault(c => c is TextContent) as TextContent;
        var content = textContent?.Text ?? string.Empty;

        _logger.LogDebug("Claude response: {Response}", content);

        try
        {
            // Extract JSON from response (Claude might include backticks or formatting)
            var jsonContent = ExtractJson(content);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<AnalysisResponse>(jsonContent, options);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize Claude response");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Claude response as JSON: {Content}", content);

            // Return a fallback response if parsing fails
            return new AnalysisResponse
            {
                EngagementScore = 50,
                Sentiment = "Neutral",
                CustomerIntent = "Browsing",
                LinkClickLikelihood = "Medium",
                NextBestMessages = new List<string>
                {
                    "Thanks for your interest! Click here to shop our latest deals: [link]",
                    "We'd love your feedback! Leave a quick review and get 10% off: [link]",
                    "Exclusive offer just for you - 20% off today only: [link]"
                },
                Insights = new List<string>
                {
                    "Unable to fully analyze conversation - please review manually"
                },
                AbTestSuggestions = new List<string>
                {
                    "Test different discount percentages (15% vs 20% vs 25%)",
                    "Test message timing (morning vs afternoon vs evening)"
                }
            };
        }
    }

    /// <summary>
    /// Extracts JSON from Claude's response, handling markdown code blocks
    /// </summary>
    private string ExtractJson(string content)
    {
        // Remove markdown code blocks if present
        content = content.Trim();

        if (content.StartsWith("```json"))
        {
            content = content.Substring(7);
        }
        else if (content.StartsWith("```"))
        {
            content = content.Substring(3);
        }

        if (content.EndsWith("```"))
        {
            content = content.Substring(0, content.Length - 3);
        }

        return content.Trim();
    }

    /// <summary>
    /// Calculates the cost of the API call based on token usage
    /// </summary>
    private decimal CalculateCost(int inputTokens, int outputTokens)
    {
        var inputCost = (inputTokens / 1_000_000m) * INPUT_COST_PER_MILLION;
        var outputCost = (outputTokens / 1_000_000m) * OUTPUT_COST_PER_MILLION;
        return inputCost + outputCost;
    }
}
