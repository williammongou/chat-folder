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
    /// Creates the Claude prompt for conversation analysis
    /// This is optimized for structured JSON output
    /// </summary>
    private string BuildAnalysisPrompt(string conversationText)
    {
        return $@"You are an expert apartment leasing consultant analyzing a conversation between a property manager and a prospective resident. Your goal is to help the property manager increase conversion rates.

CONVERSATION THREAD:
{conversationText}

Analyze this conversation and provide:

1. CONVERSION PROBABILITY (0-100): Based on engagement level, timeline urgency, objection handling, and buying signals
   - Consider: response frequency, question depth, tour scheduling, budget discussions, move-in timeline
   - High score (70-100): Strong interest, specific questions, tour scheduled, discussing move-in
   - Medium score (40-69): General interest, asking questions, comparing options
   - Low score (0-39): Browsing, vague responses, no timeline, price shopping only

2. SENTIMENT: Overall emotional tone of prospect's messages (Positive, Neutral, Negative)
   - Positive: Enthusiastic, excited, expressing needs met
   - Neutral: Professional, informational, matter-of-fact
   - Negative: Frustrated, concerned, skeptical

3. URGENCY LEVEL (Low, Medium, High): How quickly the prospect needs to move
   - High: Immediate need, specific move-in date, time pressure
   - Medium: Considering within 1-2 months, flexible timeline
   - Low: Just browsing, no specific timeline, far future

4. NEXT BEST MESSAGES: Generate exactly 3 contextually appropriate follow-up messages the property manager should send to increase conversion. These should:
   - Address prospect's specific concerns or questions
   - Move the conversation toward a tour/lease signing
   - Be natural, friendly, and professional
   - Include a clear call-to-action
   - Each message should be 2-4 sentences
   - Be personalized to this specific conversation context

5. INSIGHTS: Provide 3-5 key observations about:
   - Prospect's main pain points or objections
   - What they're interested in (amenities, location, price, etc.)
   - Opportunities to strengthen the pitch
   - Red flags or risks to conversion
   - Timeline and decision-making stage

Return your analysis as valid JSON matching this exact structure:
{{
  ""conversionProbability"": <number 0-100>,
  ""sentiment"": ""<Positive|Neutral|Negative>"",
  ""urgencyLevel"": ""<Low|Medium|High>"",
  ""nextBestMessages"": [
    ""<message 1>"",
    ""<message 2>"",
    ""<message 3>""
  ],
  ""insights"": [
    ""<insight 1>"",
    ""<insight 2>"",
    ""<insight 3>""
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
                ConversionProbability = 50,
                Sentiment = "Neutral",
                UrgencyLevel = "Medium",
                NextBestMessages = new List<string>
                {
                    "Thanks for your interest! When would be a good time for you to schedule a tour?",
                    "I'd love to answer any questions you have about our community. What's most important to you?",
                    "We have some great availability coming up. What's your ideal move-in timeline?"
                },
                Insights = new List<string>
                {
                    "Unable to fully analyze conversation - please review manually"
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
