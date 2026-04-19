# TextLiving AI Demo

**AI-Powered Apartment Leasing Conversation Analyzer**

A full-stack demo application showcasing how Claude AI can help property managers improve resident conversion through intelligent message analysis and recommendations.

![Tech Stack](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-19-DD0031?logo=angular)
![Claude](https://img.shields.io/badge/Claude-Sonnet%204.5-191919)
![Azure](https://img.shields.io/badge/Azure-Deployment-0078D4?logo=microsoftazure)

## 🎯 What This Demo Shows

This application analyzes message threads between property managers and prospective residents to provide:

1. **Conversion Probability Score** (0-100%) - How likely is this prospect to lease?
2. **Sentiment Analysis** - Is the prospect positive, neutral, or negative?
3. **Urgency Detection** - How quickly does the prospect need to move?
4. **AI-Generated Recommendations** - 3 personalized follow-up messages to increase conversion
5. **Key Insights** - Pain points, interests, objections, and opportunities

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Claude API Key](https://console.anthropic.com/) (sign up for free credits)

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd chat-folder
```

### 2. Setup Backend

```bash
cd backend/TextLivingDemo

# Install dependencies
dotnet restore

# Configure your Claude API key
# Edit appsettings.Development.json and add your key:
# "Claude": { "ApiKey": "sk-ant-your-key-here" }

# Run the API
dotnet run
```

Backend will start at `http://localhost:5000`

### 3. Setup Frontend

```bash
cd frontend/textliving-demo

# Install dependencies
npm install

# Start the dev server
npm start
```

Frontend will open at `http://localhost:4200`

### 4. Test the Demo

1. Click one of the three sample conversation buttons:
   - **High Conversion**: Engaged prospect ready to tour
   - **Medium Conversion**: Price-sensitive prospect comparing options
   - **Low Conversion**: Casual browser with no urgency

2. Click **"Analyze Conversation"**

3. Review the AI analysis:
   - Conversion score with visual progress bar
   - Sentiment and urgency indicators
   - 3 recommended follow-up messages (click to copy)
   - Detailed insights about the prospect

## 📁 Project Structure

```
chat-folder/
├── backend/
│   └── TextLivingDemo/          # .NET 8 Web API
│       ├── Controllers/         # API endpoints
│       ├── Services/            # Claude AI integration
│       ├── Models/              # Data models
│       └── README.md            # Backend documentation
│
├── frontend/
│   └── textliving-demo/         # Angular application
│       ├── src/app/             # App components
│       ├── src/environments/    # Environment configs
│       └── README-TEXTLIVING.md # Frontend documentation
│
├── DEPLOYMENT.md                # Azure deployment guide
└── README.md                    # This file
```

## 🏗️ Architecture

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   Angular   │ ──────> │  .NET API   │ ──────> │  Claude AI  │
│   Frontend  │  HTTP   │   Backend   │  HTTP   │    API      │
└─────────────┘         └─────────────┘         └─────────────┘
  localhost:4200        localhost:5000          Anthropic Cloud
```

### Tech Stack

**Frontend:**
- Angular 19 (standalone components)
- TypeScript
- RxJS for reactive programming
- Custom CSS (no heavy frameworks)
- Fully responsive design

**Backend:**
- .NET 8 Web API
- Anthropic SDK for .NET
- RESTful API design
- CORS enabled for cross-origin requests

**AI:**
- Claude Sonnet 4.5 (`claude-sonnet-4-20250514`)
- Prompt engineering for structured JSON output
- Token usage and cost tracking

**Deployment:**
- Azure App Service (Backend) - Free tier available
- Azure Static Web Apps (Frontend) - Free tier
- Total cost: ~$2-5/month for demo usage

## 🎨 Features

### User Interface
- ✅ Clean, professional design
- ✅ 3 pre-loaded sample conversations
- ✅ Real-time analysis with loading states
- ✅ Visual conversion score with progress bar
- ✅ Color-coded sentiment and urgency
- ✅ Copy-to-clipboard for recommendations
- ✅ Mobile-responsive layout
- ✅ Error handling with user-friendly messages

### Backend API
- ✅ Single endpoint design (`/api/conversation/analyze`)
- ✅ Input validation
- ✅ Structured JSON responses
- ✅ Token usage tracking
- ✅ Cost calculation
- ✅ Health check endpoint
- ✅ Comprehensive error handling
- ✅ CORS support for local dev and Azure

### AI Analysis
- ✅ Conversion probability (0-100%)
- ✅ Sentiment analysis (Positive/Neutral/Negative)
- ✅ Urgency level (High/Medium/Low)
- ✅ 3 contextual message recommendations
- ✅ Key insights extraction
- ✅ Timeline and objection identification

## 📊 Sample Results

### High Conversion Example

```
Conversion Probability: 85%
Sentiment: Positive
Urgency: High

Recommended Messages:
1. "I'm so glad Thursday at 3pm works! I'll send you a calendar invite..."
2. "Just to confirm, we have Unit 304 available with washer/dryer..."
3. "Feel free to bring your furry friend to the tour - our dog park is a favorite!"

Insights:
- Prospect has immediate timeline (end of month lease expiration)
- Specific requirements indicate serious interest
- Tour scheduled - high probability of conversion
- Budget aligns with available units
```

## 💰 Cost Information

### Claude API Pricing
- **Model**: Claude Sonnet 4.5
- **Input**: $3.00 per million tokens
- **Output**: $15.00 per million tokens
- **Typical request**: 1,000-2,000 tokens (~$0.02-$0.05)

### Azure Hosting (Monthly)
- **Backend**: $0 (Free tier) or $13 (Basic tier for production)
- **Frontend**: $0 (Free tier Static Web Apps)
- **Total**: **Free for demos**, ~$13-20/month for production

## 🚢 Deployment

Comprehensive deployment instructions are available in [DEPLOYMENT.md](DEPLOYMENT.md).

### Quick Deploy to Azure

**Backend:**
```bash
az webapp create --name textliving-api --runtime "DOTNET|8.0" --sku F1
dotnet publish -c Release
az webapp deployment source config-zip --src deploy.zip
```

**Frontend:**
```bash
ng build --configuration production
az staticwebapp create --name textliving-frontend
swa deploy --deployment-token <token>
```

See [DEPLOYMENT.md](DEPLOYMENT.md) for complete step-by-step instructions.

## 📖 Documentation

- [Backend README](backend/TextLivingDemo/README.md) - API setup and configuration
- [Frontend README](frontend/textliving-demo/README-TEXTLIVING.md) - Angular app details
- [Deployment Guide](DEPLOYMENT.md) - Azure deployment walkthrough

## 🧪 Testing the Application

### Test with Sample Conversations

The app includes three realistic scenarios:

1. **High Conversion (80-95% expected)**
   - Immediate timeline
   - Tour scheduled
   - Specific requirements
   - Budget aligned

2. **Medium Conversion (40-65% expected)**
   - Price shopping
   - Comparing multiple properties
   - 2-3 month timeline
   - Some hesitation

3. **Low Conversion (15-35% expected)**
   - Vague responses
   - No timeline
   - Minimal engagement
   - Just browsing

### Test with Custom Conversations

Format your messages as:
```
Sender Name: Message text
Another Sender: Response text
...
```

Minimum 2 messages required for analysis.

## 🛠️ Development

### Run Backend Tests
```bash
cd backend/TextLivingDemo
dotnet test
```

### Run Frontend Tests
```bash
cd frontend/textliving-demo
npm test
```

### Build for Production
```bash
# Backend
dotnet publish -c Release

# Frontend
npm run build
```

## 🐛 Troubleshooting

### Backend Issues

**Error: Claude API key not configured**
- Add your API key to `appsettings.Development.json`

**Error: CORS policy blocked**
- Verify frontend URL is in the CORS allowlist in `Program.cs`

### Frontend Issues

**Error: Cannot connect to API**
- Ensure backend is running on `http://localhost:5000`
- Check `environment.ts` has correct API URL

**Error: Module not found**
- Run `npm install` to restore dependencies

See the individual README files for more troubleshooting tips.

## 📈 Future Enhancements

Potential features for production version:

- [ ] Conversation history storage (Azure SQL/Cosmos DB)
- [ ] Multi-turn conversation analysis
- [ ] Integration with TextLiving platform
- [ ] Real-time messaging integration
- [ ] Automated follow-up scheduling
- [ ] Performance metrics dashboard
- [ ] A/B testing for message recommendations
- [ ] Bulk conversation analysis
- [ ] Export to CRM systems
- [ ] Custom training on property-specific data

## 🎯 Use Cases

This demo showcases AI capabilities for:

1. **Property Management Companies**
   - Improve lead conversion rates
   - Standardize communication quality
   - Train new leasing agents
   - Identify at-risk prospects

2. **Leasing Agents**
   - Get real-time message suggestions
   - Understand prospect urgency
   - Prioritize high-probability leads
   - Improve response quality

3. **Marketing Teams**
   - Analyze messaging effectiveness
   - Identify common objections
   - Optimize communication strategies
   - Track sentiment trends

## 🤝 Contributing

This is a demo project for interview purposes. Feel free to fork and customize for your own use.

## 📄 License

This project is for demonstration and educational purposes.

## 🙏 Acknowledgments

- Built with [Claude AI](https://www.anthropic.com/claude) by Anthropic
- Powered by [.NET 8](https://dotnet.microsoft.com/) and [Angular](https://angular.io/)
- Deployed on [Microsoft Azure](https://azure.microsoft.com/)

## 📞 Contact

For questions about this demo, please contact [your contact info].

---

**Built with ❤️ for TextLiving**

*Demonstrating the power of AI to transform apartment leasing*
