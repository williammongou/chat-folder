# TextLiving AI Demo - Backend API

.NET 8 Web API that analyzes apartment leasing conversations using Claude AI to provide conversion insights and message recommendations.

## Features

- Conversation analysis using Claude Sonnet 4.5
- Conversion probability scoring (0-100%)
- Sentiment analysis
- Urgency level detection
- AI-generated follow-up message recommendations
- Key insights extraction
- Token usage and cost tracking

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Claude API Key](https://console.anthropic.com/) (from Anthropic)

## Local Setup

### 1. Clone and Navigate

```bash
cd backend/TextLivingDemo
```

### 2. Configure API Key

Open `appsettings.Development.json` and add your Claude API key:

```json
{
  "Claude": {
    "ApiKey": "sk-ant-your-actual-api-key-here"
  }
}
```

**IMPORTANT**: Never commit your API key to version control. The `appsettings.Development.json` file should be in your `.gitignore`.

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the API

```bash
dotnet run
```

The API will start on `http://localhost:5000` (or `https://localhost:5001`).

### 5. Test the API

You can test using the Swagger UI at `http://localhost:5000/swagger` or use curl:

```bash
curl -X POST http://localhost:5000/api/conversation/analyze \
  -H "Content-Type: application/json" \
  -d '{
    "messages": [
      {"sender": "Property Manager", "text": "Hi! Thanks for your interest.", "timestamp": "2025-01-15T10:00:00Z"},
      {"sender": "Prospect", "text": "Hi, I need a 2-bedroom ASAP.", "timestamp": "2025-01-15T10:05:00Z"}
    ]
  }'
```

## Project Structure

```
TextLivingDemo/
├── Controllers/
│   └── ConversationController.cs    # API endpoint
├── Services/
│   └── ConversationAnalysisService.cs # Claude AI integration
├── Models/
│   ├── Message.cs                    # Message model
│   ├── ConversationRequest.cs        # Request DTO
│   └── AnalysisResponse.cs           # Response DTO
├── Program.cs                        # App configuration & CORS
├── appsettings.json                  # Production config
└── appsettings.Development.json      # Local dev config
```

## API Endpoints

### POST /api/conversation/analyze

Analyzes a conversation thread and returns AI insights.

**Request Body:**
```json
{
  "messages": [
    {
      "sender": "Property Manager",
      "text": "Hi! How can I help you?",
      "timestamp": "2025-01-15T10:00:00Z"
    },
    {
      "sender": "Prospect",
      "text": "Looking for a 2BR apartment",
      "timestamp": "2025-01-15T10:05:00Z"
    }
  ]
}
```

**Response:**
```json
{
  "conversionProbability": 75,
  "sentiment": "Positive",
  "urgencyLevel": "High",
  "nextBestMessages": [
    "Great! We have several 2BR units available...",
    "When are you looking to move in?",
    "Would you like to schedule a tour this week?"
  ],
  "insights": [
    "Prospect has high urgency - looking to move soon",
    "Price sensitivity is moderate",
    "Interested in specific amenities"
  ],
  "tokensUsed": 1247,
  "estimatedCost": 0.0234
}
```

### GET /api/conversation/health

Health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2025-01-15T10:00:00Z"
}
```

## Environment Variables

For production deployment, set these environment variables:

- `Claude__ApiKey` - Your Claude API key (double underscore for nested config)

Example for Azure App Service:
```bash
az webapp config appsettings set \
  --resource-group YOUR_RESOURCE_GROUP \
  --name YOUR_APP_NAME \
  --settings Claude__ApiKey="sk-ant-your-key-here"
```

## CORS Configuration

The API is configured to allow requests from:
- `http://localhost:4200` (Angular dev server)
- `https://*.azurestaticapps.net` (Azure Static Web Apps)
- `https://*.web.core.windows.net` (Azure Storage Static Sites)

To add additional origins, edit `Program.cs`:

```csharp
.WithOrigins("https://your-custom-domain.com")
```

## Building for Production

```bash
dotnet publish -c Release -o ./publish
```

This creates an optimized build in the `./publish` directory.

## Cost Information

- **Model**: Claude Sonnet 4.5 (claude-sonnet-4-20250514)
- **Input**: $3.00 per million tokens
- **Output**: $15.00 per million tokens
- **Typical analysis**: 1,000-2,000 tokens (~$0.02-$0.05 per request)

## Troubleshooting

### API Key Not Found Error

```
InvalidOperationException: Claude API key not configured
```

**Solution**: Ensure `Claude__ApiKey` is set in `appsettings.Development.json` or as an environment variable.

### CORS Error

```
Access to XMLHttpRequest has been blocked by CORS policy
```

**Solution**: Verify the frontend URL is listed in the CORS configuration in `Program.cs`.

### Claude API Rate Limits

The Anthropic API has rate limits. If you encounter 429 errors:
- Implement retry logic with exponential backoff
- Consider caching results for identical conversations
- Monitor your API usage at https://console.anthropic.com/

## Next Steps

- See [DEPLOYMENT.md](../DEPLOYMENT.md) for Azure deployment instructions
- Check the [Frontend README](../../frontend/textliving-demo/README.md) for frontend setup
