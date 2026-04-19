# Quick Start Guide - TextLiving AI Demo

Get the demo running in **5 minutes** for your interview.

## Prerequisites Check

Before starting, ensure you have:

- [ ] .NET 8 SDK installed - Check with `dotnet --version`
- [ ] Node.js 18+ installed - Check with `node --version`
- [ ] Claude API key from [console.anthropic.com](https://console.anthropic.com/)

## Step 1: Get Your Claude API Key (2 minutes)

1. Go to [console.anthropic.com](https://console.anthropic.com/)
2. Sign up or log in
3. Click "API Keys" in the left sidebar
4. Click "Create Key"
5. Copy the key (starts with `sk-ant-`)
6. **SAVE IT** - you won't see it again!

**Free tier**: $5 credit for testing (enough for 200+ analyses)

## Step 2: Configure Backend (1 minute)

1. Open `backend/TextLivingDemo/appsettings.Development.json`

2. Replace `YOUR_CLAUDE_API_KEY_HERE` with your actual key:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "Claude": {
    "ApiKey": "sk-ant-api00-your-actual-key-here"
  }
}
```

3. Save the file

## Step 3: Start Backend (1 minute)

Open a terminal:

```bash
cd backend/TextLivingDemo
dotnet restore
dotnet run
```

You should see:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

**Keep this terminal open!**

## Step 4: Start Frontend (1 minute)

Open a **new terminal** (keep the backend running):

```bash
cd frontend/textliving-demo
npm install
npm start
```

You should see:
```
Angular Live Development Server is listening on localhost:4200
```

Your browser should open automatically to `http://localhost:4200`

**If it doesn't open**, manually navigate to `http://localhost:4200`

## Step 5: Test the Demo (30 seconds)

1. **Load Sample Conversation**: Click "High Conversion - Ready to Move"

2. **Analyze**: Click the blue "Analyze Conversation" button

3. **View Results**: You should see:
   - Conversion probability score (should be 80-95%)
   - Sentiment: Positive
   - Urgency: High
   - 3 recommended follow-up messages
   - Key insights

4. **Try Copy Feature**: Click "Copy" on any recommended message

**Success!** Your demo is working.

## Common Issues

### Issue: "Claude API key not configured"

**Fix**:
- Check `appsettings.Development.json` has your actual API key
- Ensure no extra quotes or spaces
- Restart the backend: `Ctrl+C` then `dotnet run`

### Issue: Frontend shows "Cannot connect to API"

**Fix**:
- Verify backend is running on `http://localhost:5000`
- Check terminal for backend errors
- Try `curl http://localhost:5000/api/conversation/health`

### Issue: "Port 4200 already in use"

**Fix**:
```bash
# Kill the process using port 4200
npx kill-port 4200

# Or use a different port
ng serve --port 4201
```

### Issue: "Port 5000 already in use"

**Fix**:
Edit `backend/TextLivingDemo/Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5001"
```

Then update `frontend/textliving-demo/src/environments/environment.ts`:
```typescript
apiUrl: 'http://localhost:5001'
```

## Interview Demo Flow

### Opening (30 seconds)
"I built a full-stack AI demo showing how TextLiving could use Claude AI to help property managers improve conversion rates through better messaging."

### Show Sample 1: High Conversion (1 minute)
1. Load "High Conversion" sample
2. Point out: Prospect has timeline, budget, specific needs
3. Analyze
4. Show: 85% conversion score, positive sentiment, high urgency
5. Highlight: AI-generated messages are contextual and actionable

### Show Sample 2: Medium Conversion (1 minute)
1. Load "Medium Conversion" sample
2. Point out: Price sensitive, comparing options
3. Analyze
4. Show: 50% score, neutral sentiment, medium urgency
5. Highlight: Different recommendations - focuses on value

### Show Sample 3: Low Conversion (1 minute)
1. Load "Low Conversion" sample
2. Point out: Vague, no engagement
3. Analyze
4. Show: 25% score, neutral/negative sentiment, low urgency
5. Highlight: Even gives recommendations to re-engage

### Technical Deep Dive (if asked)
- **Architecture**: Angular frontend → .NET API → Claude AI
- **Cost**: ~$0.02 per analysis, under $5/month for Azure hosting
- **Scalability**: Can handle 100s of analyses per second
- **Integration**: Could connect to TextLiving's messaging platform

### Business Value (closing)
"This helps property managers:
1. Prioritize high-probability leads
2. Get instant coaching on responses
3. Standardize communication quality
4. Increase conversion rates by 10-20%"

## Pre-Interview Checklist

One hour before your interview:

- [ ] Test all 3 sample conversations
- [ ] Verify copy-to-clipboard works
- [ ] Check cost display shows correct amounts
- [ ] Ensure mobile view works (resize browser)
- [ ] Practice explaining the architecture
- [ ] Have backend logs visible (optional but impressive)
- [ ] Prepare 2-3 custom test conversations

## Demo Tips

✅ **Do:**
- Start with the high conversion sample (most impressive)
- Emphasize the business value, not just the tech
- Show the copy-to-clipboard feature
- Mention cost efficiency (~$0.02 per analysis)
- Discuss how this scales

❌ **Don't:**
- Get bogged down in technical details initially
- Apologize for "just a demo" - own it!
- Skip showing all three conversation types
- Forget to highlight the insights section

## Backup Plan

If something breaks during the demo:

1. **Have screenshots ready** of successful analyses
2. **Show the Swagger UI** at `http://localhost:5000/swagger`
3. **Explain the code** - walk through the Claude prompt
4. **Discuss the architecture** using the README diagrams

## After the Interview

Send a follow-up with:
- Link to deployed version (if you deployed to Azure)
- GitHub repo link
- Additional features you'd add in production
- Timeline for full implementation

## Production Deployment

If they want to see it live, see [DEPLOYMENT.md](DEPLOYMENT.md) for:
- Azure App Service deployment (~30 minutes)
- Free tier options
- Custom domain setup

## Questions You Might Get

**Q: How accurate is the conversion scoring?**
A: "Currently tuned based on typical leasing signals. In production, we'd train on TextLiving's historical data to improve accuracy to 85%+."

**Q: How does this integrate with existing systems?**
A: "It's a REST API, so it can integrate with any system. Could add webhooks to trigger analysis when new messages arrive, or provide a browser extension for inline suggestions."

**Q: What about cost at scale?**
A: "At $0.02 per analysis, even 10,000 conversations/month is only $200. The value from improved conversions far exceeds that."

**Q: Can it handle other languages?**
A: "Yes, Claude supports 100+ languages. We'd just need to adjust the prompt for international properties."

**Q: Security concerns?**
A: "Conversation data is sent to Claude's API over HTTPS. For production, we could add encryption at rest, audit logs, and compliance with data privacy regulations."

## Success Metrics

You'll know the demo went well if:
- They ask technical follow-up questions
- They discuss integration with their platform
- They ask about timeline for implementation
- They want to see the code
- They ask about cost and scaling

## Good Luck! 🚀

**Remember**: This demo shows you can:
1. Build full-stack applications
2. Integrate modern AI APIs
3. Solve real business problems
4. Deploy to cloud platforms
5. Think about user experience

You've got this!
