# Azure Deployment Guide - TextLiving AI Demo

Complete step-by-step guide for deploying the TextLiving AI Demo to Azure for **under $5/month**.

## Cost Breakdown (Estimated Monthly)

| Service | Tier | Cost |
|---------|------|------|
| Azure Static Web Apps (Frontend) | Free | $0.00 |
| Azure App Service (Backend) | F1 Free | $0.00 |
| Claude API Usage | Pay-as-you-go | ~$2-5 |
| **Total** | | **~$2-5/month** |

**Note**: The Free tier has limitations (60 min/day compute for App Service). For production, consider Basic tier ($13/month).

## Prerequisites

1. **Azure Account**: [Create free account](https://azure.microsoft.com/free/) ($200 credit for 30 days)
2. **Azure CLI**: [Install Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
3. **GitHub Account**: For Static Web Apps deployment (optional but recommended)
4. **Claude API Key**: From [Anthropic Console](https://console.anthropic.com/)

## Part 1: Deploy Backend (.NET API)

### Option A: Azure App Service (Recommended for Demo)

#### Step 1: Login to Azure

```bash
az login
```

#### Step 2: Create Resource Group

```bash
az group create \
  --name textliving-demo-rg \
  --location eastus
```

#### Step 3: Create App Service Plan (Free Tier)

```bash
az appservice plan create \
  --name textliving-demo-plan \
  --resource-group textliving-demo-rg \
  --sku F1 \
  --is-linux
```

**Note**: F1 (Free) tier limitations:
- 60 minutes/day compute time
- 1 GB disk space
- No custom domains
- Perfect for demos and interviews

For production, upgrade to B1 (Basic):
```bash
--sku B1  # $13/month, always-on, better performance
```

#### Step 4: Create Web App

```bash
az webapp create \
  --resource-group textliving-demo-rg \
  --plan textliving-demo-plan \
  --name textliving-api-demo \
  --runtime "DOTNET|8.0"
```

**IMPORTANT**: The name `textliving-api-demo` must be globally unique. If taken, try:
- `textliving-api-demo-yourname`
- `textliving-api-demo-123`

Your API URL will be: `https://textliving-api-demo.azurewebsites.net`

#### Step 5: Configure API Settings

Set the Claude API key:

```bash
az webapp config appsettings set \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo \
  --settings Claude__ApiKey="sk-ant-your-actual-api-key-here"
```

Enable detailed error messages (for debugging only, remove in production):

```bash
az webapp config appsettings set \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo \
  --settings ASPNETCORE_ENVIRONMENT="Development"
```

#### Step 6: Build and Publish Backend

From your local machine:

```bash
cd backend/TextLivingDemo

# Build release version
dotnet publish -c Release -o ./publish

# Create deployment ZIP
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo \
  --src deploy.zip
```

**Windows PowerShell alternative**:
```powershell
Compress-Archive -Path .\publish\* -DestinationPath deploy.zip -Force
az webapp deployment source config-zip `
  --resource-group textliving-demo-rg `
  --name textliving-api-demo `
  --src deploy.zip
```

#### Step 7: Verify Backend Deployment

Test the health endpoint:

```bash
curl https://textliving-api-demo.azurewebsites.net/api/conversation/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-15T10:00:00Z"
}
```

Check Swagger UI:
```
https://textliving-api-demo.azurewebsites.net/swagger
```

### Option B: Azure Container Apps (Alternative - More Expensive)

**Cost**: ~$15-20/month minimum

Only use this if you need:
- Auto-scaling
- Container-based deployment
- Better performance guarantees

```bash
# Create Container Apps environment
az containerapp env create \
  --name textliving-env \
  --resource-group textliving-demo-rg \
  --location eastus

# Deploy container (requires Docker image)
# See Microsoft docs for full steps
```

## Part 2: Deploy Frontend (Angular)

### Option A: Azure Static Web Apps (Recommended - FREE)

#### Step 1: Build Frontend for Production

Update the API URL in `frontend/textliving-demo/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://textliving-api-demo.azurewebsites.net'  // Your API URL from Part 1
};
```

Build the production bundle:

```bash
cd frontend/textliving-demo
npm run build
```

Output will be in: `dist/textliving-demo/browser/`

#### Step 2: Create Static Web App

```bash
az staticwebapp create \
  --name textliving-frontend \
  --resource-group textliving-demo-rg \
  --location eastus2
```

This creates a Static Web App and returns a deployment token.

#### Step 3: Deploy Frontend Files

Using Azure CLI:

```bash
# Install SWA CLI
npm install -g @azure/static-web-apps-cli

# Deploy
cd dist/textliving-demo/browser
swa deploy --deployment-token="<your-deployment-token-from-step-2>"
```

**Alternative: Manual Upload via Azure Portal**

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to your Static Web App resource
3. Click "Browse" to get your URL
4. Use the portal's file upload feature to upload files from `dist/textliving-demo/browser/`

#### Step 4: Configure CORS in Backend

Update the backend to allow your frontend URL:

```bash
# Get your Static Web App URL
az staticwebapp show \
  --name textliving-frontend \
  --resource-group textliving-demo-rg \
  --query "defaultHostname" -o tsv
```

This returns something like: `nice-rock-0a1b2c3d4.2.azurestaticapps.net`

The backend's `Program.cs` already includes wildcards for `*.azurestaticapps.net`, so CORS should work automatically.

#### Step 5: Test the Full Application

Visit your Static Web App URL:
```
https://nice-rock-0a1b2c3d4.2.azurestaticapps.net
```

1. Load a sample conversation
2. Click "Analyze Conversation"
3. Verify results appear correctly

### Option B: Azure Storage Static Website (Cheapest - ~$0.50/month)

#### Step 1: Create Storage Account

```bash
az storage account create \
  --name textlivingdemo \
  --resource-group textliving-demo-rg \
  --location eastus \
  --sku Standard_LRS \
  --kind StorageV2
```

**Note**: Storage account name must be lowercase, 3-24 characters, globally unique.

#### Step 2: Enable Static Website Hosting

```bash
az storage blob service-properties update \
  --account-name textlivingdemo \
  --static-website \
  --index-document index.html \
  --404-document index.html
```

#### Step 3: Get Storage Account Key

```bash
az storage account keys list \
  --account-name textlivingdemo \
  --resource-group textliving-demo-rg \
  --query "[0].value" -o tsv
```

#### Step 4: Upload Files

```bash
cd frontend/textliving-demo/dist/textliving-demo/browser

az storage blob upload-batch \
  --account-name textlivingdemo \
  --source . \
  --destination '$web' \
  --account-key "<key-from-step-3>"
```

#### Step 5: Get Website URL

```bash
az storage account show \
  --name textlivingdemo \
  --resource-group textliving-demo-rg \
  --query "primaryEndpoints.web" -o tsv
```

URL format: `https://textlivingdemo.z13.web.core.windows.net/`

The backend's CORS configuration already includes `*.web.core.windows.net`, so it should work.

## Part 3: Post-Deployment Configuration

### Update Frontend API URL (If Changed)

If your backend URL changed after deployment:

1. Update `src/environments/environment.prod.ts` with correct URL
2. Rebuild: `npm run build`
3. Redeploy frontend files

### Enable HTTPS and Custom Domain (Optional)

#### For Static Web Apps:

Static Web Apps include free HTTPS automatically. For custom domains:

```bash
az staticwebapp hostname set \
  --name textliving-frontend \
  --resource-group textliving-demo-rg \
  --hostname www.yourdomain.com
```

Requires DNS configuration. See [Microsoft docs](https://docs.microsoft.com/azure/static-web-apps/custom-domain).

#### For App Service:

Free tier doesn't support custom domains. Upgrade to Basic tier:

```bash
az appservice plan update \
  --name textliving-demo-plan \
  --resource-group textliving-demo-rg \
  --sku B1
```

### Monitor Costs

Check your Azure costs:

```bash
az consumption usage list \
  --start-date 2025-01-01 \
  --end-date 2025-01-31
```

Or use the [Azure Portal Cost Management](https://portal.azure.com/#blade/Microsoft_Azure_CostManagement/Menu/overview).

### Set Up Monitoring (Optional)

Enable Application Insights for the backend:

```bash
az monitor app-insights component create \
  --app textliving-insights \
  --location eastus \
  --resource-group textliving-demo-rg \
  --application-type web

# Get instrumentation key
az monitor app-insights component show \
  --app textliving-insights \
  --resource-group textliving-demo-rg \
  --query "instrumentationKey" -o tsv

# Configure App Service to use it
az webapp config appsettings set \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY="<key-from-above>"
```

## Part 4: Updating Your Deployment

### Update Backend

```bash
cd backend/TextLivingDemo

# Make your code changes
# ...

# Rebuild and redeploy
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip .
cd ..

az webapp deployment source config-zip \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo \
  --src deploy.zip
```

### Update Frontend

```bash
cd frontend/textliving-demo

# Make your code changes
# ...

# Rebuild
npm run build

# Redeploy (Static Web Apps)
cd dist/textliving-demo/browser
swa deploy --deployment-token="<your-token>"

# OR Redeploy (Storage)
az storage blob upload-batch \
  --account-name textlivingdemo \
  --source . \
  --destination '$web' \
  --account-key "<your-key>" \
  --overwrite
```

## Part 5: CI/CD with GitHub Actions (Optional)

### Backend CI/CD

Create `.github/workflows/backend-deploy.yml`:

```yaml
name: Deploy Backend to Azure

on:
  push:
    branches: [ main ]
    paths:
      - 'backend/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Build and Publish
        run: |
          cd backend/TextLivingDemo
          dotnet publish -c Release -o ./publish

      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'textliving-api-demo'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: backend/TextLivingDemo/publish
```

### Frontend CI/CD

Create `.github/workflows/frontend-deploy.yml`:

```yaml
name: Deploy Frontend to Azure Static Web Apps

on:
  push:
    branches: [ main ]
    paths:
      - 'frontend/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Build
        run: |
          cd frontend/textliving-demo
          npm ci
          npm run build

      - name: Deploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "frontend/textliving-demo"
          output_location: "dist/textliving-demo/browser"
```

## Troubleshooting

### Backend Not Starting

Check logs:
```bash
az webapp log tail \
  --resource-group textliving-demo-rg \
  --name textliving-api-demo
```

Common issues:
- Missing Claude API key
- Wrong .NET runtime version
- Startup errors in code

### CORS Errors

If frontend can't reach backend:

1. Verify CORS settings in `Program.cs` include your frontend URL
2. Restart the App Service after changes
3. Check browser console for exact error

### Static Web App 404 Errors

Angular apps need URL rewriting. Create `staticwebapp.config.json` in `frontend/textliving-demo/src/`:

```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["*.{css,scss,js,png,gif,ico,jpg,svg}"]
  }
}
```

Then rebuild and redeploy.

### High Claude API Costs

Monitor usage at [Anthropic Console](https://console.anthropic.com/):
- Set usage limits
- Review token consumption per request
- Consider caching frequent queries

## Cleanup (Delete Everything)

To remove all Azure resources and stop charges:

```bash
az group delete \
  --name textliving-demo-rg \
  --yes \
  --no-wait
```

This deletes:
- App Service
- Storage Account
- Static Web App
- All associated resources

## Cost Optimization Tips

1. **Free Tier is Sufficient for Demos**: F1 App Service + Free Static Web Apps = $0
2. **Monitor Claude API Usage**: Set alerts at $5, $10, $20
3. **Use Basic (B1) for Real Traffic**: Only $13/month, much better performance
4. **Storage is Cheaper**: Static website hosting costs ~$0.02/GB/month
5. **Turn Off When Not in Use**: Stop the App Service between demos

## Next Steps for Production

When moving beyond the demo:

1. **Upgrade App Service**: F1 → B1 or S1 for reliability
2. **Add Authentication**: Azure AD, Auth0, or custom JWT
3. **Enable Caching**: Redis cache for frequently analyzed conversations
4. **Add Rate Limiting**: Protect against abuse
5. **Custom Domain**: Professional appearance
6. **CDN**: Azure CDN for faster global delivery
7. **Database**: Store conversation history with Azure SQL or Cosmos DB

## Support Resources

- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service/)
- [Azure Static Web Apps Docs](https://docs.microsoft.com/azure/static-web-apps/)
- [Anthropic API Docs](https://docs.anthropic.com/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/)

## Summary Checklist

- [ ] Backend deployed to Azure App Service
- [ ] Claude API key configured
- [ ] Backend health check returns 200 OK
- [ ] Frontend built with production config
- [ ] Frontend deployed to Static Web Apps or Storage
- [ ] CORS configured correctly
- [ ] Full end-to-end test completed
- [ ] Costs monitored and within budget
- [ ] Documentation reviewed and understood

**Your demo is ready!** 🎉
