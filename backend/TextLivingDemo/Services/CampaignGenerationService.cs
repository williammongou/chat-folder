using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using TextLivingDemo.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace TextLivingDemo.Services;

public class CampaignGenerationService
{
    private readonly AnthropicClient _anthropicClient;
    private readonly ILogger<CampaignGenerationService> _logger;
    private static int _totalTokensToday = 0;

    public CampaignGenerationService(IConfiguration configuration, ILogger<CampaignGenerationService> logger)
    {
        var apiKey = configuration["Claude:ApiKey"]
            ?? throw new InvalidOperationException("Claude API key not configured");

        _anthropicClient = new AnthropicClient(apiKey);
        _logger = logger;
    }

    public async Task<CampaignGenerationResponse> GenerateCampaignAsync(CampaignGenerationRequest request)
    {
        try
        {
            var prompt = BuildCampaignPrompt(request);

            var parameters = new MessageParameters
            {
                Messages = new List<Anthropic.SDK.Messaging.Message>
                {
                    new Anthropic.SDK.Messaging.Message(RoleType.User, prompt)
                },
                MaxTokens = 3000,
                Model = "claude-sonnet-4-20250514",
                Stream = false,
                Temperature = 0.8m
            };

            var response = await _anthropicClient.Messages.GetClaudeMessageAsync(parameters);

            var tokensUsed = response.Usage.OutputTokens + response.Usage.InputTokens;
            _totalTokensToday += tokensUsed;

            var messageContent = response.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;

            var campaignResponse = ParseCampaignResponse(messageContent, tokensUsed);

            return campaignResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating campaign");
            return GetFallbackResponse();
        }
    }

    public TokenUsageResponse GetTokenUsage()
    {
        var percentage = (_totalTokensToday / 10000m) * 100;
        var status = percentage < 50 ? "green" : percentage < 80 ? "orange" : "red";

        return new TokenUsageResponse
        {
            TokensUsedToday = _totalTokensToday,
            TokenLimit = 10000,
            PercentageUsed = percentage,
            Status = status
        };
    }

    public void ResetTokenUsage()
    {
        _totalTokensToday = 0;
    }

    private string BuildCampaignPrompt(CampaignGenerationRequest request)
    {
        var promptBuilder = new System.Text.StringBuilder();

        promptBuilder.AppendLine("You are an expert SMS marketing campaign generator for TextLiving.");
        promptBuilder.AppendLine("TextLiving helps brick-and-mortar businesses (restaurants, car washes, smoothie shops, etc.) send promotional SMS campaigns to drive foot traffic and in-store conversions.");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine($"USER REQUEST: {request.Prompt}");
        promptBuilder.AppendLine();

        if (request.HistoricalData?.Campaigns?.Any() == true)
        {
            promptBuilder.AppendLine("HISTORICAL CAMPAIGN DATA:");
            promptBuilder.AppendLine($"Business: {request.HistoricalData.BusinessName} ({request.HistoricalData.BusinessType})");
            promptBuilder.AppendLine();

            var bestCampaigns = request.HistoricalData.Campaigns
                .OrderByDescending(c => c.ConversionRate)
                .Take(2)
                .ToList();

            var worstCampaigns = request.HistoricalData.Campaigns
                .OrderByDescending(c => c.UnsubscribeRate)
                .Take(2)
                .ToList();

            promptBuilder.AppendLine("TOP PERFORMING CAMPAIGNS:");
            foreach (var campaign in bestCampaigns)
            {
                promptBuilder.AppendLine($"  - \"{campaign.OutgoingText}\"");
                promptBuilder.AppendLine($"    Conversion Rate: {campaign.ConversionRate}%, Unsubscribe Rate: {campaign.UnsubscribeRate}%");
                promptBuilder.AppendLine($"    Foot Traffic: {campaign.FootTraffic}/{campaign.RecipientCount}");

                var positiveReplies = campaign.IncomingReplies
                    .Where(r => !r.Text.Equals("STOP", StringComparison.OrdinalIgnoreCase) &&
                               !r.Text.Equals("QUIT", StringComparison.OrdinalIgnoreCase) &&
                               !r.Text.Contains("Unsubscribe", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (positiveReplies.Any())
                {
                    promptBuilder.AppendLine($"    Positive replies: {string.Join(", ", positiveReplies.Select(r => $"\"{r.Text}\" ({r.Count})"))}");
                }
                promptBuilder.AppendLine();
            }

            promptBuilder.AppendLine("WORST PERFORMING CAMPAIGNS (AVOID THESE PATTERNS):");
            foreach (var campaign in worstCampaigns)
            {
                promptBuilder.AppendLine($"  - \"{campaign.OutgoingText}\"");
                promptBuilder.AppendLine($"    Conversion Rate: {campaign.ConversionRate}%, Unsubscribe Rate: {campaign.UnsubscribeRate}%");

                var unsubscribeReplies = campaign.IncomingReplies
                    .Where(r => r.Text.Equals("STOP", StringComparison.OrdinalIgnoreCase) ||
                               r.Text.Equals("QUIT", StringComparison.OrdinalIgnoreCase) ||
                               r.Text.Contains("Unsubscribe", StringComparison.OrdinalIgnoreCase) ||
                               r.Text.Contains("spam", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (unsubscribeReplies.Any())
                {
                    promptBuilder.AppendLine($"    Unsubscribe replies: {string.Join(", ", unsubscribeReplies.Select(r => $"\"{r.Text}\" ({r.Count})"))}");
                }
                promptBuilder.AppendLine($"    Why it failed: {campaign.Notes}");
                promptBuilder.AppendLine();
            }

            if (request.HistoricalData.Insights != null)
            {
                promptBuilder.AppendLine("KEY INSIGHTS:");
                promptBuilder.AppendLine($"  Best send time: {request.HistoricalData.Insights.BestSendTime}");

                if (request.HistoricalData.Insights.RedFlags.Any())
                {
                    promptBuilder.AppendLine($"  Red flags to avoid: {string.Join(", ", request.HistoricalData.Insights.RedFlags)}");
                }

                if (request.HistoricalData.Insights.SuccessPatterns.Any())
                {
                    promptBuilder.AppendLine($"  Success patterns: {string.Join(", ", request.HistoricalData.Insights.SuccessPatterns)}");
                }
                promptBuilder.AppendLine();
            }
        }

        promptBuilder.AppendLine("TASK:");
        promptBuilder.AppendLine("Generate exactly 2 promotional SMS campaign options that will drive foot traffic and conversions.");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("REQUIREMENTS:");
        promptBuilder.AppendLine("- Each message must be under 160 characters (SMS limit)");
        promptBuilder.AppendLine("- Include clear call-to-action");
        promptBuilder.AppendLine("- Specify deadline or urgency (today, until 9PM, etc.)");
        promptBuilder.AppendLine("- Use friendly, not aggressive tone");
        promptBuilder.AppendLine("- Avoid red flags from historical data (all caps, too many exclamation marks, vague offers)");
        promptBuilder.AppendLine("- Incorporate success patterns from historical data");
        promptBuilder.AppendLine("- Make Option 1 and Option 2 meaningfully different approaches");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Return response as JSON:");
        promptBuilder.AppendLine(@"{
  ""option1"": {
    ""messageText"": ""<SMS text under 160 chars>"",
    ""whyThisWorks"": ""<2-3 sentence explanation>"",
    ""bestSendTime"": ""<specific time like '11:30 AM' or '5:00 PM'>"",
    ""insights"": [""<insight 1>"", ""<insight 2>"", ""<insight 3>""],
    ""predictedConversionRate"": <number like 5.2>,
    ""toneAnalysis"": ""<friendly/urgent/personalized/etc>""
  },
  ""option2"": {
    ""messageText"": ""<SMS text under 160 chars>"",
    ""whyThisWorks"": ""<2-3 sentence explanation>"",
    ""bestSendTime"": ""<specific time>"",
    ""insights"": [""<insight 1>"", ""<insight 2>"", ""<insight 3>""],
    ""predictedConversionRate"": <number>,
    ""toneAnalysis"": ""<tone>""
  }
}");

        return promptBuilder.ToString();
    }

    private CampaignGenerationResponse ParseCampaignResponse(string response, int tokensUsed)
    {
        try
        {
            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonContent = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed != null)
                {
                    var option1 = ParseCampaignOption(parsed["option1"]);
                    var option2 = ParseCampaignOption(parsed["option2"]);

                    const decimal inputCostPer1M = 3.0m;
                    const decimal outputCostPer1M = 15.0m;
                    var estimatedCost = (tokensUsed / 1_000_000m) * ((inputCostPer1M + outputCostPer1M) / 2);

                    return new CampaignGenerationResponse
                    {
                        Option1 = option1,
                        Option2 = option2,
                        TokensUsed = tokensUsed,
                        TotalTokensToday = _totalTokensToday,
                        EstimatedCost = estimatedCost
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing campaign response");
        }

        return GetFallbackResponse();
    }

    private CampaignOption ParseCampaignOption(JsonElement element)
    {
        return new CampaignOption
        {
            MessageText = element.GetProperty("messageText").GetString() ?? string.Empty,
            WhyThisWorks = element.GetProperty("whyThisWorks").GetString() ?? string.Empty,
            BestSendTime = element.GetProperty("bestSendTime").GetString() ?? string.Empty,
            Insights = element.GetProperty("insights").EnumerateArray()
                .Select(i => i.GetString() ?? string.Empty).ToList(),
            PredictedConversionRate = element.GetProperty("predictedConversionRate").GetDecimal(),
            ToneAnalysis = element.GetProperty("toneAnalysis").GetString() ?? string.Empty
        };
    }

    private CampaignGenerationResponse GetFallbackResponse()
    {
        return new CampaignGenerationResponse
        {
            Option1 = new CampaignOption
            {
                MessageText = "Special offer today! Visit us and get 20% off. Show this text at checkout. Valid until 9 PM.",
                WhyThisWorks = "Clear offer with specific discount, deadline creates urgency, simple redemption process.",
                BestSendTime = "11:00 AM",
                Insights = new List<string> { "Includes specific discount percentage", "Clear deadline", "Easy to redeem" },
                PredictedConversionRate = 4.5m,
                ToneAnalysis = "Friendly and clear"
            },
            Option2 = new CampaignOption
            {
                MessageText = "Thanks for being a loyal customer! Here's a free item on your next visit. Valid for 7 days.",
                WhyThisWorks = "Personalized appreciation, free item is strong incentive, reasonable timeframe reduces pressure.",
                BestSendTime = "10:00 AM",
                Insights = new List<string> { "Personalized message", "Free item offer", "Multi-day validity" },
                PredictedConversionRate = 6.2m,
                ToneAnalysis = "Appreciative and warm"
            },
            TokensUsed = 0,
            TotalTokensToday = _totalTokensToday,
            EstimatedCost = 0
        };
    }
}
