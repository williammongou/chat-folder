# How to Use the TextLiving SMS Campaign Analyzer

## Step-by-Step Guide

### 1. Start the Backend API

Open **Terminal 1** (Command Prompt or PowerShell):

```bash
cd backend/TextLivingDemo
dotnet run
```

**You should see:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

Keep this terminal open! ✅

---

### 2. Start the Frontend

Open **Terminal 2** (new Command Prompt or PowerShell window):

```bash
cd frontend/textliving-demo
npm start
```

**You should see:**
```
✔ Browser application bundle generation complete.
Angular Live Development Server is listening on localhost:4200
```

**Browser should auto-open to:** `http://localhost:4200`

If it doesn't, manually go to: **http://localhost:4200**

---

### 3. Using the Interface

You should see:

```
┌─────────────────────────────────────────────┐
│  TextLiving SMS Campaign Analyzer           │
│  AI-Powered SMS Marketing Analytics         │
└─────────────────────────────────────────────┘

SMS Conversation Thread
Paste an SMS conversation between your business and customer

[High Engagement - Flash Sale] [Medium Engagement - Review Request] [Low Engagement - Promotional Fatigue]

┌────────────────────────────────────────────┐
│ Business: Flash Sale! 25% off...          │
│ Customer: Omg yes!...                      │
│ Business: Perfect! Code FLASH25...         │
│                                             │
│                                             │
└────────────────────────────────────────────┘

[Analyze Campaign]  [Clear]
```

---

### 4. Analyze a Conversation (3 Ways)

#### Option A: Use Sample Conversations (Easiest!)

1. **Click one of the three buttons** at the top:
   - "High Engagement - Flash Sale"
   - "Medium Engagement - Review Request"
   - "Low Engagement - Promotional Fatigue"

2. This **automatically loads** the conversation into the text box

3. Click the big blue **"Analyze Campaign"** button

4. Wait 2-3 seconds for AI analysis

5. **Results appear on the right side!**

#### Option B: Paste Your Own Conversation

1. **Copy this sample** (or use your own):
```
Business: Hey! Flash Sale - 30% OFF everything today only! Shop now: shop.co/flash30
Customer: Omg perfect timing! I need new shoes
Business: Great! Use code FLASH30 at checkout. Free shipping over $50!
Customer: Just ordered! Thanks!
```

2. **Paste it** into the text box (the big gray box)

3. Click **"Analyze Campaign"**

4. Wait for results!

#### Option C: Type Your Own

1. **Type** in the format:
```
Sender: Message text
Sender: Message text
```

Example:
```
Business: Sale alert! 50% off
Customer: Interested!
Business: Click here: link.co/sale
Customer: Just clicked it
```

2. Click **"Analyze Campaign"**

---

### 5. Understanding the Results

After clicking "Analyze Campaign", you'll see on the **right side**:

#### Engagement Score (Big Number)
```
┌─────────────────────┐
│  Engagement Score   │
│       85%           │  ← Green = High (70-100%)
│  ████████████████   │     Yellow = Medium (40-69%)
└─────────────────────┘     Red = Low (0-39%)
```

#### Three Metrics
```
┌──────────┬──────────────┬─────────────┐
│Sentiment │Customer Intent│Click Likelihood│
│ Positive │  Interested  │    High     │
└──────────┴──────────────┴─────────────┘
```

#### Optimized Follow-up Messages
```
┌────────────────────────────────────────┐
│ 1  Hey! Thanks for your order! Here's │  [Copy]
│    a 15% code for next time: SAVE15    │
├────────────────────────────────────────┤
│ 2  We just restocked the items you    │  [Copy]
│    liked! Shop now with free shipping  │
├────────────────────────────────────────┤
│ 3  Quick question - would you leave   │  [Copy]
│    a review? Get 10% off: review.co/1  │
└────────────────────────────────────────┘
```

Click **[Copy]** to copy any message to your clipboard!

#### Key Insights (Bullet Points)
- Customer has high purchase intent
- Responded quickly to flash sale
- Has history of clicking links
- etc.

#### A/B Testing Recommendations 🧪 (NEW!)
```
🧪 Test discount variation: 25% vs 30% vs 40%
🧪 Test message timing: 10am vs 2pm vs 6pm
🧪 Test CTA wording: "Shop Now" vs "Get Deal"
```

---

### 6. Troubleshooting

#### "I don't see the buttons!"

**Solution 1: Hard refresh the browser**
- Windows/Linux: `Ctrl + Shift + R`
- Mac: `Cmd + Shift + R`

**Solution 2: Clear browser cache**
- Open DevTools (F12)
- Right-click the refresh button
- Click "Empty Cache and Hard Reload"

**Solution 3: Restart frontend**
```bash
# In Terminal 2, press Ctrl+C to stop
# Then run again:
npm start
```

#### "Analyze Campaign button is grayed out"

This happens when:
- Text box is empty → **Load a sample or type something**
- Already analyzing → **Wait a few seconds**

#### "Nothing happens when I click Analyze"

**Check these:**

1. **Backend running?**
   - Look at Terminal 1
   - Should say "Now listening on: http://localhost:5000"
   - If not, restart it: `dotnet run`

2. **Open browser console** (F12)
   - Look for errors in red
   - Common: "Failed to fetch" = backend not running
   - Common: "CORS error" = restart backend

3. **Claude API key set?**
   - Open `backend/TextLivingDemo/appsettings.Development.json`
   - Make sure `"ApiKey": "sk-ant-..."` has your real key
   - Restart backend after adding key

#### "Error: Failed to analyze conversation"

**Red error box appears:**

**Most common cause:** Missing Claude API key

**Fix:**
1. Go to [console.anthropic.com](https://console.anthropic.com/)
2. Sign up / Log in
3. Click "API Keys" → "Create Key"
4. Copy the key (starts with `sk-ant-`)
5. Open `backend/TextLivingDemo/appsettings.Development.json`
6. Replace `YOUR_CLAUDE_API_KEY_HERE` with your actual key
7. **Restart the backend** (Ctrl+C in Terminal 1, then `dotnet run`)
8. Try again!

#### "The page looks broken / old apartment text"

The frontend didn't reload the new changes.

**Fix:**
```bash
# Terminal 2 - Stop frontend (Ctrl+C)
cd frontend/textliving-demo
npm start
```

Then hard refresh browser (`Ctrl + Shift + R`)

---

### 7. Quick Test (30 Seconds)

1. ✅ Backend running? (Terminal 1)
2. ✅ Frontend running? (Terminal 2)
3. ✅ Browser open? (`http://localhost:4200`)
4. ✅ Click **"High Engagement - Flash Sale"** button
5. ✅ Click blue **"Analyze Campaign"** button
6. ✅ Wait 2-3 seconds
7. ✅ See results on right side with 85% engagement score!

If step 7 works → **You're all set!** 🎉

---

### 8. Demo Flow for Interview

**30-second demo:**

1. "Let me show you three different customer scenarios"

2. Click **"High Engagement - Flash Sale"**
   - "This customer is excited, clicked the link, made a purchase"
   - Click **Analyze**
   - Point to **85% engagement** score
   - "AI detects high intent, suggests upsell messages"

3. Click **"Low Engagement - Promotional Fatigue"**
   - "This customer is getting too many texts"
   - Click **Analyze**
   - Point to **25% engagement**, "Opted-Out" intent
   - "AI detects the problem and suggests reducing frequency"
   - **Show A/B testing suggestions**

4. Click **[Copy]** on a message
   - "Messages are optimized for SMS, ready to send"

**Key points to mention:**
- Real-time analysis (< 3 seconds)
- Detects opt-out risk before customer leaves
- Suggests A/B tests to improve campaigns
- Costs ~$0.02 per analysis

---

### 9. Visual Guide

```
┌────────────────────────────────────────────────────────────────────┐
│                    TextLiving SMS Campaign Analyzer                 │
│                AI-Powered SMS Marketing Analytics                   │
└────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────┬─────────────────────────────────────┐
│  INPUT SECTION              │  RESULTS SECTION                    │
│                              │  (appears after clicking Analyze)   │
│  [Sample Button 1] ← Click! │                                     │
│  [Sample Button 2]          │  ┌───────────────────────────┐     │
│  [Sample Button 3]          │  │ Engagement Score: 85%     │     │
│                              │  │ ████████████████ (green)  │     │
│  ┌────────────────────────┐ │  └───────────────────────────┘     │
│  │ Text box shows         │ │                                     │
│  │ conversation here      │ │  ┌──┬─────────────┬──────────┐     │
│  │ after clicking sample  │ │  │  │   Intent    │  Click   │     │
│  │                        │ │  └──┴─────────────┴──────────┘     │
│  │                        │ │                                     │
│  └────────────────────────┘ │  ┌──────────────────────────┐     │
│                              │  │ 1 Suggested message...   │     │
│  [Analyze Campaign] ← Click! │  │ 2 Another message...     │     │
│  [Clear]                    │  │ 3 Third message...       │     │
│                              │  └──────────────────────────┘     │
│                              │                                     │
│                              │  🧪 A/B Test Suggestions:          │
│                              │  • Test discount %                 │
│                              │  • Test timing                     │
└─────────────────────────────┴─────────────────────────────────────┘
```

---

## Still Stuck?

**Check this in order:**

1. ✅ Backend Terminal shows "Now listening on: http://localhost:5000"
2. ✅ Frontend Terminal shows "Angular Live Development Server is listening on localhost:4200"
3. ✅ Browser is at `http://localhost:4200` (not 5000!)
4. ✅ You see "TextLiving SMS Campaign Analyzer" header
5. ✅ You see three sample buttons
6. ✅ You see a text box
7. ✅ You see "Analyze Campaign" button
8. ✅ Claude API key is in `appsettings.Development.json`

If all 8 are true and it still doesn't work:
- Press F12 in browser
- Look for errors in Console tab
- Share the error message

---

**You've got this! The interface is simple:**
1. **Click a sample button** (or type your own)
2. **Click "Analyze Campaign"**
3. **See results!**

🚀
