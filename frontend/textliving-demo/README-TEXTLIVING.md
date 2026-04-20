# TextLiving AI Demo - Frontend

Angular application for analyzing apartment leasing conversations with AI-powered insights.

## Features

- Clean, professional UI with responsive design
- 3 pre-loaded sample conversations (high/medium/low conversion)
- Real-time conversation analysis
- Visual conversion probability score with progress bar
- Color-coded sentiment and urgency indicators
- AI-generated message recommendations with copy-to-clipboard
- Key insights display
- Token usage and cost tracking
- Mobile-friendly layout

## Prerequisites

- [Node.js 18+](https://nodejs.org/) (LTS recommended)
- [Angular CLI](https://angular.io/cli) - installed automatically with npm

## Local Setup

### 1. Navigate to Project

```bash
cd frontend/textliving-demo
```

### 2. Install Dependencies

```bash
npm install
```

### 3. Configure API URL

The default configuration points to `http://localhost:5000` for the backend API.

To change this, edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'  // Change if your backend is on a different port
};
```

### 4. Start Development Server

```bash
npm start
```

Or use Angular CLI directly:

```bash
ng serve
```

The app will open at `http://localhost:4200` and will automatically reload when you make changes.

### 5. Use the Application

1. **Load a Sample**: Click one of the three sample conversation buttons
2. **Analyze**: Click "Analyze Conversation" button
3. **View Results**: See conversion probability, sentiment, urgency, recommendations, and insights
4. **Copy Messages**: Click "Copy" on any recommended message to copy it to clipboard
5. **Try Custom**: Paste your own conversation in the textarea (format: "Sender: Message text")

## Project Structure

```
src/
├── app/
│   ├── models/
│   │   └── message.model.ts         # TypeScript interfaces
│   ├── services/
│   │   └── conversation.service.ts  # API communication service
│   ├── app.ts                        # Main component logic
│   ├── app.html                      # Main component template
│   ├── app.css                       # Component styling
│   └── app.config.ts                 # App configuration (HttpClient)
├── environments/
│   ├── environment.ts                # Development config
│   └── environment.prod.ts           # Production config
├── index.html                        # HTML entry point
├── main.ts                           # Bootstrap file
└── styles.css                        # Global styles
```

## Sample Conversations

The app includes three realistic apartment leasing scenarios:

### 1. High Conversion - Ready to Move
- Prospect has immediate timeline (end of month)
- Specific requirements (2BR, W/D, pet-friendly)
- Tour scheduled
- **Expected Score**: 80-95%

### 2. Medium Conversion - Price Sensitive
- Prospect is price shopping
- Considering multiple properties
- 2-3 month timeline
- **Expected Score**: 40-65%

### 3. Low Conversion - Just Browsing
- Vague responses
- No specific timeline
- Minimal engagement
- **Expected Score**: 15-35%

## Building for Production

### Create Production Build

```bash
npm run build
```

Or:

```bash
ng build --configuration production
```

This creates optimized files in the `dist/textliving-demo/browser` directory.

### Build Output Location

```
dist/textliving-demo/browser/
├── index.html
├── main-[hash].js
├── polyfills-[hash].js
├── styles-[hash].css
└── ...
```

## Configuration

### Environment-Specific Settings

**Development** (`environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

**Production** (`environment.prod.ts`):
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api.azurewebsites.net'  // Update with your Azure API URL
};
```

### API Service

The `ConversationService` handles all API communication:

```typescript
// Analyze a conversation
this.conversationService.analyzeConversation({ messages })
  .subscribe({
    next: (result) => {
      // Handle success
    },
    error: (error) => {
      // Handle error
    }
  });
```

## Customization

### Change Color Scheme

Edit CSS variables in `src/app/app.css`:

```css
:root {
  --primary-color: #2563eb;       /* Main brand color */
  --success-color: #10b981;       /* High conversion */
  --warning-color: #f59e0b;       /* Medium conversion */
  --danger-color: #ef4444;        /* Low conversion */
  /* ... more variables ... */
}
```

### Add More Sample Conversations

Edit `src/app/app.ts`:

```typescript
sampleConversations = [
  {
    name: 'Your Sample Name',
    text: `Sender 1: Message 1
Sender 2: Message 2
...`
  },
  // ... existing samples
];
```

### Modify Conversion Score Thresholds

Edit the `getConversionClass()` method in `src/app/app.ts`:

```typescript
getConversionClass(score: number): string {
  if (score >= 70) return 'conversion-high';     // Change threshold
  if (score >= 40) return 'conversion-medium';   // Change threshold
  return 'conversion-low';
}
```

## Troubleshooting

### Error: Cannot connect to API

**Symptoms**: "Failed to analyze conversation. Please check your API connection."

**Solutions**:
1. Verify backend API is running (`dotnet run` in backend directory)
2. Check API URL in `environment.ts` matches your backend
3. Ensure CORS is properly configured in the backend
4. Check browser console for detailed error messages

### CORS Error in Browser Console

```
Access to XMLHttpRequest at 'http://localhost:5000' from origin 'http://localhost:4200' has been blocked
```

**Solution**: Add `http://localhost:4200` to the CORS origins in the backend's `Program.cs`.

### Build Errors

```
Error: Module not found
```

**Solution**: Delete `node_modules` and reinstall:
```bash
rm -rf node_modules package-lock.json
npm install
```

### TypeScript Errors

Ensure you're using a compatible TypeScript version:
```bash
npm install typescript@latest --save-dev
```

## Testing

### Run Unit Tests

```bash
npm test
```

Or:

```bash
ng test
```

### Run in Production Mode Locally

```bash
ng serve --configuration production
```

This tests the production build configuration without deploying.

## Performance Optimization

The app is already optimized with:
- Lazy loading for Angular modules
- AOT (Ahead-of-Time) compilation in production
- Minification and tree-shaking
- Gzip compression ready

Typical bundle sizes:
- **main.js**: ~150-200 KB (gzipped)
- **Total load**: < 300 KB (gzipped)
- **First load**: < 1 second on 3G

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Next Steps

- See [DEPLOYMENT.md](../DEPLOYMENT.md) for Azure deployment instructions
- Check the [Backend README](../../backend/TextLivingDemo/README.md) for API setup
- Review Angular documentation at [angular.io](https://angular.io)

## Demo Tips for Interview

1. **Start with samples**: Show all three conversation types to demonstrate AI accuracy
2. **Highlight features**: Copy message to clipboard, show cost tracking
3. **Explain the value**: Point out how this saves property managers time
4. **Show mobile**: Resize browser to show responsive design
5. **Discuss scale**: Mention how this could integrate with TextLiving's existing platform



# Kill already running process:
### ** use this command: taskkill /F /IM TextLivingDemo.exe