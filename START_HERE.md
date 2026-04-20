# START HERE - Getting the Demo Running

## The Port Issue (IMPORTANT!)

The backend API will run on **http://localhost:5000** after the fix I just made.

### Quick Fix Applied ✅

I've already updated:
1. `launchSettings.json` - Changed port from 5073 to 5000
2. `Program.cs` - Disabled HTTPS redirection for local dev

### How to Start the Backend

Open a terminal in the project root and run:

```bash
cd backend/TextLivingDemo
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Test It's Working

In a new terminal (or browser), test the health endpoint:

```bash
curl http://localhost:5000/api/conversation/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2026-04-19T..."
}
```

Or open in browser:
```
http://localhost:5000/swagger
```

You should see the Swagger UI with the API documentation.

---

## Frontend Setup

In a **NEW terminal** (keep backend running):

```bash
cd frontend/textliving-demo
npm install     # First time only
npm start
```

Browser should open to `http://localhost:4200`

---

## If You Still Get Port Errors

### Issue: "Port 5000 already in use"

**Windows - Kill process on port 5000:**
```powershell
Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process -Force
```

Or use a different port:
```bash
dotnet run --urls "http://localhost:5001"
```

Then update `frontend/textliving-demo/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5001'  // Changed from 5000
};
```

### Issue: "Connection Refused" or "ERR_CONNECTION_REFUSED"

This means the backend isn't running. Make sure:
1. Backend terminal is open and running
2. You see "Now listening on: http://localhost:5000"
3. No errors in the backend terminal

### Issue: CORS Error

If you see: `Access to XMLHttpRequest blocked by CORS policy`

**Solution**: Backend already has CORS configured for `http://localhost:4200`. Just make sure:
1. Backend is running
2. You're accessing frontend from `http://localhost:4200` (not a different port)

---

## Quick Test (30 seconds)

Once both are running:

1. Go to `http://localhost:4200`
2. Click **"High Conversion - Ready to Move"**
3. Click **"Analyze Conversation"**
4. You should see results with ~85% conversion score!

---

## Before Your Claude API Key Setup

The app won't analyze conversations without a Claude API key. Here's how to get one:

### Get Claude API Key (2 minutes)

1. Go to [console.anthropic.com](https://console.anthropic.com/)
2. Sign up or log in
3. Click "API Keys" in left sidebar
4. Click "Create Key"
5. Copy the key (starts with `sk-ant-`)

### Add API Key to Backend

Edit `backend/TextLivingDemo/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "Claude": {
    "ApiKey": "sk-ant-api00-YOUR-ACTUAL-KEY-HERE"
  }
}
```

**Important**: Replace `YOUR-ACTUAL-KEY-HERE` with your real API key!

Then restart the backend:
- Press `Ctrl+C` in the backend terminal
- Run `dotnet run` again

---

## Troubleshooting Backend

### Build Errors

```bash
cd backend/TextLivingDemo
dotnet clean
dotnet build
```

Look for errors in the output.

### Can't Find API Key

Error: `InvalidOperationException: Claude API key not configured`

**Fix**:
1. Check `appsettings.Development.json` has your API key
2. Make sure there are no extra quotes or spaces
3. Restart the backend

### Swagger Shows But API Fails

If Swagger UI loads but API calls fail:
- Check backend terminal for error logs
- Look for red error messages
- Common issue: Missing Claude API key

---

## Troubleshooting Frontend

### npm install Fails

```bash
cd frontend/textliving-demo
rm -rf node_modules package-lock.json
npm install
```

### Port 4200 Already in Use

**Option 1 - Kill the process:**
```bash
npx kill-port 4200
npm start
```

**Option 2 - Use different port:**
```bash
ng serve --port 4201
```

Then open `http://localhost:4201` in your browser.

### "Cannot connect to API" Error

1. Verify backend is running: `curl http://localhost:5000/api/conversation/health`
2. Check `src/environments/environment.ts` has correct API URL
3. Look in browser console (F12) for detailed errors

---

## Full Restart (If Everything is Broken)

1. **Stop everything:**
   - Close all terminals
   - Kill any running processes

2. **Clean rebuild backend:**
   ```bash
   cd backend/TextLivingDemo
   dotnet clean
   dotnet build
   dotnet run
   ```

3. **In NEW terminal, restart frontend:**
   ```bash
   cd frontend/textliving-demo
   npm start
   ```

4. **Test:**
   - Backend: `http://localhost:5000/swagger`
   - Frontend: `http://localhost:4200`
   - Run a sample analysis

---

## Success Checklist

- [ ] Backend running on port 5000
- [ ] Swagger UI loads at http://localhost:5000/swagger
- [ ] Health endpoint returns `{"status":"healthy"}`
- [ ] Frontend loads at http://localhost:4200
- [ ] Can load sample conversations
- [ ] Analysis works (with Claude API key configured)
- [ ] Results display correctly

---

## Next Steps

Once it's running, check out:
- [QUICK_START.md](QUICK_START.md) - Demo preparation
- [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) - Demo strategy
- [DEMO_CHECKLIST.txt](DEMO_CHECKLIST.txt) - Interview day checklist

---

## Still Stuck?

1. Check the backend terminal for error messages
2. Check browser console (F12 → Console tab)
3. Try the full restart procedure above
4. Review the error messages - they usually tell you what's wrong

**Most common issues:**
- Missing Claude API key
- Port already in use
- Backend not running when you start frontend
- Wrong API URL in frontend config

You've got this! 🚀
