# Claude AI Prompt Engineering Guide

This document explains the prompt used in the TextLiving AI Demo and how to optimize it for reliable, structured responses.

## The Prompt Template

Located in: `backend/TextLivingDemo/Services/ConversationAnalysisService.cs`

### Full Prompt Structure

```
You are an expert apartment leasing consultant analyzing a conversation between a
property manager and a prospective resident. Your goal is to help the property
manager increase conversion rates.

CONVERSATION THREAD:
[conversation text here]

Analyze this conversation and provide:

1. CONVERSION PROBABILITY (0-100): Based on engagement level, timeline urgency,
   objection handling, and buying signals

2. SENTIMENT: Overall emotional tone (Positive, Neutral, Negative)

3. URGENCY LEVEL (Low, Medium, High): How quickly the prospect needs to move

4. NEXT BEST MESSAGES: Generate exactly 3 contextually appropriate follow-up
   messages...

5. INSIGHTS: Provide 3-5 key observations...

Return your analysis as valid JSON matching this exact structure:
{
  "conversionProbability": <number 0-100>,
  "sentiment": "<Positive|Neutral|Negative>",
  "urgencyLevel": "<Low|Medium|High>",
  "nextBestMessages": ["<message 1>", "<message 2>", "<message 3>"],
  "insights": ["<insight 1>", "<insight 2>", "<insight 3>"]
}

IMPORTANT: Return ONLY the JSON object, no additional text or formatting.
```

## Why This Prompt Works

### 1. Clear Role Definition

```
You are an expert apartment leasing consultant...
```

**Why it matters**: Sets the context and expertise level. Claude responds with domain-specific knowledge about leasing, sales psychology, and conversion optimization.

### 2. Explicit Goal Statement

```
Your goal is to help the property manager increase conversion rates.
```

**Why it matters**: Aligns the AI's recommendations toward actionable business outcomes rather than generic analysis.

### 3. Structured Input Format

```
CONVERSATION THREAD:
[formatted messages]
```

**Why it matters**: Provides clear, consistent input format that Claude can parse reliably.

### 4. Detailed Criteria for Each Output

```
1. CONVERSION PROBABILITY (0-100): Based on engagement level, timeline urgency,
   objection handling, and buying signals
```

**Why it matters**: Gives Claude specific factors to consider, ensuring consistent, justified scoring.

### 5. Constrained Output Values

```
"sentiment": "<Positive|Neutral|Negative>"
```

**Why it matters**: Limits responses to expected values, making parsing reliable and preventing unexpected outputs.

### 6. Exact JSON Structure

```json
{
  "conversionProbability": <number>,
  "sentiment": "<string>",
  ...
}
```

**Why it matters**: Claude understands the exact schema needed, reducing parsing errors.

### 7. Explicit Instruction for Format

```
IMPORTANT: Return ONLY the JSON object, no additional text or formatting.
```

**Why it matters**: Prevents Claude from adding explanatory text, code blocks, or markdown that would break JSON parsing.

## Prompt Engineering Best Practices

### ✅ Do's

1. **Be Specific About Output Format**
   ```
   Return as valid JSON
   Use this exact structure
   Return ONLY the JSON object
   ```

2. **Provide Examples of Desired Behavior**
   ```
   High score (70-100): Strong interest, tour scheduled
   Medium score (40-69): General interest, comparing options
   Low score (0-39): Browsing, no timeline
   ```

3. **Use Constraints to Reduce Variability**
   ```
   exactly 3 messages
   3-5 key observations
   Positive|Neutral|Negative (not other values)
   ```

4. **Set Context and Expertise**
   ```
   You are an expert [role]
   Your goal is to [objective]
   ```

5. **Break Down Complex Tasks**
   ```
   1. CONVERSION PROBABILITY...
   2. SENTIMENT...
   3. URGENCY LEVEL...
   ```

### ❌ Don'ts

1. **Don't Be Vague**
   ```
   ❌ "Analyze this conversation"
   ✅ "Provide conversion probability (0-100), sentiment (Positive/Neutral/Negative)..."
   ```

2. **Don't Allow Open-Ended Outputs**
   ```
   ❌ "sentiment": "somewhat positive but with concerns"
   ✅ "sentiment": "Neutral"
   ```

3. **Don't Forget to Handle Edge Cases**
   ```
   - What if conversation is only 1 message?
   - What if messages are in foreign language?
   - What if conversation is hostile?
   ```

4. **Don't Overcomplicate the Prompt**
   - Keep it focused on the task
   - Remove unnecessary context
   - Use clear, direct language

## Handling Claude's Response

### Parsing Strategy

The `ConversationAnalysisService.cs` handles several response formats:

```csharp
private string ExtractJson(string content)
{
    // Remove markdown code blocks if present
    content = content.Trim();

    if (content.StartsWith("```json"))
        content = content.Substring(7);
    else if (content.StartsWith("```"))
        content = content.Substring(3);

    if (content.EndsWith("```"))
        content = content.Substring(0, content.Length - 3);

    return content.Trim();
}
```

**Why this matters**: Claude sometimes wraps JSON in markdown code blocks despite instructions. This ensures we can parse it regardless.

### Fallback Responses

```csharp
catch (JsonException ex)
{
    // Return a safe, generic response
    return new AnalysisResponse
    {
        ConversionProbability = 50,
        Sentiment = "Neutral",
        NextBestMessages = new List<string> { "Generic message 1", ... }
    };
}
```

**Why this matters**: If parsing fails, provide a reasonable default rather than crashing.

## Token Optimization

### Current Prompt Size

- **Prompt template**: ~800 tokens
- **Typical conversation**: 200-400 tokens
- **Expected response**: 400-600 tokens
- **Total per request**: ~1,400-1,800 tokens

### Cost Per Request

```
Input:  1,000 tokens × $3.00 / 1M = $0.003
Output:   500 tokens × $15.00 / 1M = $0.0075
Total: ~$0.0105 per request
```

### Optimization Strategies

1. **Shorten the Prompt (If Needed)**
   - Remove examples once Claude is performing well
   - Consolidate instructions
   - Use abbreviations for field names

2. **Limit Conversation Length**
   ```csharp
   var messages = allMessages.TakeLast(10); // Only last 10 messages
   ```

3. **Use Lower-Tier Models for Simple Cases**
   - Claude Haiku for basic sentiment only
   - Claude Sonnet for full analysis (current)
   - Claude Opus for complex, nuanced conversations

4. **Cache Prompts (Future Enhancement)**
   - Anthropic supports prompt caching
   - Can reduce costs by 90% for repeated prompt portions

## Testing Different Prompt Variations

### Experiment Ideas

**Test 1: Simplified Prompt**
Remove examples and see if accuracy drops:
```
Analyze conversation. Return JSON with conversionProbability (0-100),
sentiment (Positive/Neutral/Negative), urgencyLevel (Low/Medium/High),
3 nextBestMessages, and 3-5 insights.
```

**Test 2: Add Examples**
Provide one-shot learning examples:
```
Example conversation:
Prospect: I need a place ASAP
Manager: We have availability!

Output:
{"conversionProbability": 80, "urgencyLevel": "High", ...}
```

**Test 3: Different Model Comparison**
```csharp
Model = "claude-haiku-20250514"  // Faster, cheaper
Model = "claude-opus-20250514"   // Most capable
```

**Test 4: Temperature Adjustment**
```csharp
Temperature = 0.0   // More deterministic
Temperature = 0.5   // Default
Temperature = 1.0   // More creative
```

### Measuring Prompt Performance

Create test cases with expected outputs:

```csharp
[Fact]
public async Task AnalyzeConversation_HighConversion_ReturnsHighScore()
{
    var messages = GetHighConversionSample();
    var result = await _service.AnalyzeConversationAsync(messages);

    Assert.InRange(result.ConversionProbability, 70, 100);
    Assert.Equal("Positive", result.Sentiment);
    Assert.Equal("High", result.UrgencyLevel);
}
```

## Advanced Prompt Techniques

### Chain-of-Thought Prompting

Add reasoning steps:
```
First, identify key signals:
- Timeline mentions
- Budget discussions
- Tour requests

Then, calculate probability based on...
```

### Few-Shot Learning

Provide multiple examples:
```
Example 1:
Input: [conversation A]
Output: [analysis A]

Example 2:
Input: [conversation B]
Output: [analysis B]

Now analyze:
Input: [actual conversation]
Output: ...
```

### Constitutional AI

Add ethical guidelines:
```
Important guidelines:
- Never discriminate based on protected classes
- Be objective about conversion probability
- Focus on helping both parties find good fit
```

## Prompt Version Control

Track prompt changes and performance:

```csharp
public class ConversationAnalysisService
{
    private const string PROMPT_VERSION = "v2.1";

    // Include version in logs
    _logger.LogInformation("Using prompt version {Version}", PROMPT_VERSION);
}
```

## Monitoring and Iteration

### Metrics to Track

1. **Response Quality**
   - Parsing success rate
   - JSON validation errors
   - Fallback usage frequency

2. **Cost Metrics**
   - Average tokens per request
   - Cost per analysis
   - Daily/monthly spend

3. **Business Metrics**
   - Recommendation acceptance rate (if tracked)
   - User satisfaction with suggestions
   - Actual conversion rate correlation

### Continuous Improvement

1. **Collect edge cases** that produce poor results
2. **Refine prompt** to handle those cases
3. **A/B test** different prompt versions
4. **Measure impact** on quality and cost
5. **Deploy winner** and repeat

## Example Prompt Evolution

### Version 1.0 (Initial)
```
Analyze this conversation and tell me if they'll convert.
```
**Problem**: Too vague, inconsistent format

### Version 2.0 (Structured)
```
Analyze conversation. Return: probability, sentiment, recommendations.
```
**Problem**: No format specification, varied outputs

### Version 2.5 (JSON Specified)
```
Analyze and return JSON: {"probability": number, "sentiment": string, ...}
```
**Problem**: Inconsistent values (e.g., "somewhat positive")

### Version 3.0 (Current - Constrained)
```
Return JSON with exact structure. Values must be: Positive|Neutral|Negative...
```
**Success**: Reliable, parseable, consistent

## Resources

- [Anthropic Prompt Engineering Guide](https://docs.anthropic.com/claude/docs/prompt-engineering)
- [Claude API Reference](https://docs.anthropic.com/claude/reference/)
- [Prompt Caching Documentation](https://docs.anthropic.com/claude/docs/prompt-caching)

## Summary

The prompt in this demo is optimized for:
- ✅ Reliable JSON output
- ✅ Consistent value constraints
- ✅ Domain-specific expertise
- ✅ Actionable recommendations
- ✅ Low token usage (~1,500 per request)
- ✅ Clear business alignment

**Key Takeaway**: Successful prompts are specific, structured, and constrained. Always test with real data and iterate based on results.
