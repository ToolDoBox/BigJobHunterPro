# Big Job Hunter Pro - Azure Deployment Plan (v1.0)

## Overview

Deploy Big Job Hunter Pro MVP to Azure with:

- **Backend**: .NET 8 API on Azure App Service
- **Frontend**: React SPA on Azure Static Web Apps
- **Database**: Existing Azure SQL (bjhp-dev-sql-cgta.database.windows.net)
- **Secrets**: Azure Key Vault with Managed Identity
- **CI/CD**: GitHub Actions pipelines

**Estimated Time**: 2-3 hours
**Estimated Cost**: ~$15-18/month

---

## Phase 1: Azure Resource Provisioning (30 min)

### 1.1 Create Azure Key Vault

**Purpose**: Securely store secrets (DB connection, JWT secret, Anthropic API key)

```bash
az keyvault create \
  --name bjhp-keyvault-prod \
  --resource-group <your-resource-group> \
  --location <your-region>
```

### 1.2 Store Secrets in Key Vault

```bash
# Database connection string (get from existing Azure SQL)
az keyvault secret set \
  --vault-name bjhp-keyvault-prod \
  --name ConnectionStrings--DefaultConnection \
  --value "Server=bjhp-dev-sql-cgta.database.windows.net;Database=BigJobHunterPro;User Id=<admin>;Password=<password>;Encrypt=True;TrustServerCertificate=False"

# JWT Secret (generate a strong random 256-bit key)
az keyvault secret set \
  --vault-name bjhp-keyvault-prod \
  --name JwtSettings--Secret \
  --value "<generate-strong-256-bit-key>"

# Anthropic API Key (from appsettings.json line 12)
az keyvault secret set \
  --vault-name bjhp-keyvault-prod \
  --name AnthropicSettings--ApiKey \
  --value "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"
```

### 1.3 Create App Service (Backend)

```bash
# Create App Service Plan (B1 tier)
az appservice plan create \
  --name bjhp-appservice-plan \
  --resource-group <your-resource-group> \
  --sku B1 \
  --is-linux

# Create App Service
az webapp create \
  --name bjhp-api-prod \
  --resource-group <your-resource-group> \
  --plan bjhp-appservice-plan \
  --runtime "DOTNET|8.0"

# Enable HTTPS only
az webapp update \
  --name bjhp-api-prod \
  --resource-group <your-resource-group> \
  --https-only true
```

### 1.4 Enable Managed Identity & Grant Key Vault Access

```bash
# Enable system-assigned managed identity
az webapp identity assign \
  --name bjhp-api-prod \
  --resource-group <your-resource-group>

# Copy the principalId from output, then:
az keyvault set-policy \
  --name bjhp-keyvault-prod \
  --object-id <principal-id-from-above> \
  --secret-permissions get list
```

### 1.5 Configure Azure SQL Firewall

```bash
# Get App Service outbound IPs
az webapp show \
  --name bjhp-api-prod \
  --resource-group <your-resource-group> \
  --query possibleOutboundIpAddresses -o tsv

# Add each IP to SQL Server firewall in Azure Portal:
# SQL Server (bjhp-dev-sql-cgta) > Networking > Firewall rules > Add client IP
```

### 1.6 Create Static Web App (Frontend)

**Via Azure Portal** (easier for initial setup):

1. Navigate to: Azure Portal → Static Web Apps → Create
2. Configure:
   - **Name**: bjhp-frontend-prod
   - **Region**: Same as resource group
   - **Source**: GitHub
   - **Organization**: ToolDoBox
   - **Repository**: BigJobHunterPro
   - **Branch**: main
   - **Build Preset**: React
   - **App location**: /bigjobhunterpro-web
   - **Output location**: dist
3. Click "Review + Create"

**Result**: Auto-generates `.github/workflows/azure-static-web-apps-*.yml` in your repo

### 1.7 Create Application Insights (Optional but Recommended)

```bash
az monitor app-insights component create \
  --app bjhp-appinsights-prod \
  --location <your-region> \
  --resource-group <your-resource-group> \
  --application-type web

# Copy the connectionString from output
```

---

## Phase 2: Backend Code Changes (30 min)

### 2.1 Create appsettings.Production.json

**File**: `src/WebAPI/appsettings.Production.json` (NEW FILE)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "** LOADED FROM AZURE KEY VAULT **"
  },
  "JwtSettings": {
    "Secret": "** LOADED FROM AZURE KEY VAULT **",
    "Issuer": "BigJobHunterPro",
    "Audience": "BigJobHunterPro",
    "ExpirationDays": 7
  },
  "AnthropicSettings": {
    "ApiKey": "** LOADED FROM AZURE KEY VAULT **",
    "Model": "claude-haiku-4-5",
    "MaxTokens": 1024,
    "TimeoutSeconds": 30,
    "PollingIntervalSeconds": 5
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2.2 Secure appsettings.json - Remove Hardcoded API Key

**File**: `src/WebAPI/appsettings.json`

**Change line 12** from:

```json
"ApiKey": "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",
```

To:

```json
"ApiKey": "** PLACEHOLDER - Use User Secrets for local dev, Azure Key Vault for production **",
```

**CRITICAL**: This prevents accidental credential leaks in version control.

### 2.3 Update Program.cs - Add SQL Server Retry Logic

**File**: `src/WebAPI/Program.cs`

**Replace lines 18-29** with:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "bigjobhunterpro.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    else
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    }
});
```

### 2.4 Update Program.cs - Production CORS Configuration

**File**: `src/WebAPI/Program.cs`

**Find the CORS configuration** (around line 112-122) and replace with:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")
            .Get<string[]>()
            ?? new[] { "http://localhost:5173", "http://localhost:5174" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### 2.5 Add Application Insights (Optional)

**File**: `src/WebAPI/WebAPI.csproj`

Add package reference:

```xml
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.22.0" />
```

**File**: `src/WebAPI/Program.cs`

Add after line 82 (after AddControllers):

```csharp
// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();
```

---

## Phase 3: Frontend Code Changes (15 min)

### 3.1 Create Production Environment File

**File**: `bigjobhunterpro-web/.env.production` (NEW FILE)

```env
VITE_API_URL=https://bjhp-api-prod.azurewebsites.net
VITE_APP_NAME=Big Job Hunter Pro
```

### 3.2 Create Static Web App Configuration

**File**: `bigjobhunterpro-web/public/staticwebapp.config.json` (NEW FILE)

```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": [
      "/images/*",
      "/*.{css,scss,js,json,png,jpg,jpeg,gif,svg,ico,woff,woff2,ttf}"
    ]
  },
  "globalHeaders": {
    "X-Content-Type-Options": "nosniff",
    "X-Frame-Options": "DENY",
    "Strict-Transport-Security": "max-age=31536000"
  }
}
```

---

## Phase 4: Configure Azure App Service Settings (15 min)

### 4.1 Add Application Settings

**Navigate to**: Azure Portal → App Service (bjhp-api-prod) → Configuration → Application settings

**Add these settings**:

```json
{
  "ASPNETCORE_ENVIRONMENT": "Production",
  "KeyVaultName": "bjhp-keyvault-prod",

  // Key Vault references (these pull from Key Vault automatically)
  "ConnectionStrings__DefaultConnection": "@Microsoft.KeyVault(VaultName=bjhp-keyvault-prod;SecretName=ConnectionStrings--DefaultConnection)",
  "JwtSettings__Secret": "@Microsoft.KeyVault(VaultName=bjhp-keyvault-prod;SecretName=JwtSettings--Secret)",
  "AnthropicSettings__ApiKey": "@Microsoft.KeyVault(VaultName=bjhp-keyvault-prod;SecretName=AnthropicSettings--ApiKey)",

  // JWT non-secret settings
  "JwtSettings__Issuer": "BigJobHunterPro",
  "JwtSettings__Audience": "BigJobHunterPro",
  "JwtSettings__ExpirationDays": "7",

  // Anthropic non-secret settings
  "AnthropicSettings__Model": "claude-haiku-4-5",
  "AnthropicSettings__MaxTokens": "1024",
  "AnthropicSettings__TimeoutSeconds": "30",
  "AnthropicSettings__PollingIntervalSeconds": "5",

  // CORS - Update after Static Web App is deployed
  "AllowedOrigins__0": "https://bjhp-frontend-prod.azurestaticapps.net",

  // Application Insights (if created in 1.7)
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "<connection-string-from-step-1.7>"
}
```

**Click "Save"** at the top of the page.

### 4.2 Configure General Settings

**Navigate to**: Configuration → General settings

- **Always On**: On (prevents cold starts)
- **HTTPS Only**: On
- **Minimum TLS Version**: 1.2
- **Health check path**: /api/health

**Click "Save"**.

---

## Phase 5: Database Migration (15 min)

### Option A: Manual Migration (Recommended for v1.0)

**From your local machine**:

```bash
cd E:\Dev\BigJobHunterPro\src\WebAPI

# Set connection string temporarily
$env:ConnectionStrings__DefaultConnection = "Server=bjhp-dev-sql-cgta.database.windows.net;Database=BigJobHunterPro;User Id=<admin>;Password=<password>;Encrypt=True;TrustServerCertificate=False"

# Run migrations
dotnet ef database update

# Verify migrations applied
dotnet ef migrations list
```

### Option B: Verify in Azure SQL

**Connect to Azure SQL via Azure Portal**:

1. Azure Portal → SQL Database (BigJobHunterPro) → Query editor
2. Login with admin credentials
3. Run:

```sql
SELECT * FROM __EFMigrationsHistory;
-- Should show: 20260105055544_InitialIdentity and 20260119090000_AddApplicationsTable

SELECT COUNT(*) FROM AspNetUsers;
SELECT COUNT(*) FROM Applications;
```

!!!! Double Check !!!!

Failed to execute query. Error: Invalid object name '\_\_EFMigrationsHistory'.

!!!!!!!!

---

## Phase 6: Backend Deployment (20 min)

### 6.1 Create GitHub Actions Workflow

**File**: `.github/workflows/deploy-backend.yml` (NEW FILE)

```yaml
name: Deploy Backend to Azure

on:
  push:
    branches: [main]
    paths:
      - "src/**"
      - ".github/workflows/deploy-backend.yml"
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: bjhp-api-prod
  DOTNET_VERSION: "8.0.x"

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore BigJobHunterPro.sln

      - name: Build
        run: dotnet build BigJobHunterPro.sln --configuration Release --no-restore

      - name: Publish
        run: dotnet publish src/WebAPI/WebAPI.csproj --configuration Release --no-build --output ./publish

      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish
```

### 6.2 Add GitHub Secret

1. **Download Publish Profile**:

   - Azure Portal → App Service (bjhp-api-prod) → Get publish profile (download)
   - Open the downloaded `.PublishSettings` file in a text editor

2. **Add to GitHub**:
   - GitHub → Repository → Settings → Secrets and variables → Actions
   - New repository secret:
     - **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
     - **Value**: Paste entire contents of `.PublishSettings` file

### 6.3 Commit and Push

```bash
git add .
git commit -m "feat: add Azure deployment configuration"
git push origin main
```

**Monitor deployment**: GitHub → Actions tab → "Deploy Backend to Azure" workflow

---

## Phase 7: Frontend Deployment (Auto-configured)

### 7.1 Update Static Web App Environment Variables

**Navigate to**: Azure Portal → Static Web App (bjhp-frontend-prod) → Configuration

**Add environment variables**:

- **Name**: `VITE_API_URL`
- **Value**: `https://bjhp-api-prod.azurewebsites.net`

**Note**: The Static Web App workflow was auto-created when you set it up in Phase 1.6. It deploys automatically on push to main.

### 7.2 Update Backend CORS with Static Web App URL

**After Static Web App is deployed**, get its URL:

- Azure Portal → Static Web App → Overview → Copy URL (e.g., `https://bjhp-frontend-prod-xyz.azurestaticapps.net`)

**Update Backend App Service**:

- Azure Portal → App Service (bjhp-api-prod) → Configuration → Application settings
- Update `AllowedOrigins__0` to the Static Web App URL
- **Save**

---

## Phase 8: Post-Deployment Validation (30 min)

### 8.1 Backend Health Check

**Test health endpoint**:

```bash
curl https://bjhp-api-prod.azurewebsites.net/api/health
# Expected: {"status":"ok","timestampUtc":"..."}

curl https://bjhp-api-prod.azurewebsites.net/api/health/db
# Expected: {"status":"ok","database":"connected",...}
```

### 8.2 Test Authentication

```bash
# Test registration
curl -X POST https://bjhp-api-prod.azurewebsites.net/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "displayName": "Test User"
  }'

# Expected: 200 OK with JWT token
```

### 8.3 Frontend Validation

1. **Open browser**: Navigate to Static Web App URL
2. **Register**: Create a new account
3. **Login**: Login with credentials
4. **Create Application**: Use Quick Capture to add a job application
5. **Verify Points**: Check that points are awarded (+1 for application)

### 8.4 Check Key Vault Integration

**Azure Portal → App Service → Configuration**:

- Verify Key Vault references show **green checkmark** ✓
- If red X, check Managed Identity permissions

**Check logs**:

```bash
# Azure Portal → App Service → Log stream
# Look for any "KeyVault" or authentication errors
```

### 8.5 Verify CORS

**In browser**:

1. Open DevTools → Network tab
2. Login to frontend
3. Watch API calls
4. Verify no CORS errors in console

---

## Phase 9: Monitoring Setup (Optional - 15 min)

### 9.1 Create Alerts

**Azure Portal → Application Insights → Alerts**:

**Alert 1: High Error Rate**

- Signal: Failed requests
- Threshold: > 10 in 5 minutes
- Action: Email notification

**Alert 2: High Response Time**

- Signal: Server response time
- Threshold: > 2 seconds (P95)
- Action: Email notification

**Alert 3: Database Issues**

- Signal: Dependency failures
- Threshold: > 5 in 5 minutes
- Action: Email notification

### 9.2 Set Budget Alert

```bash
az consumption budget create \
  --budget-name bjhp-monthly-budget \
  --amount 25 \
  --time-grain Monthly \
  --resource-group <your-resource-group>
```

---

## Summary of Changes Required

### New Files to Create:

1. ✅ `src/WebAPI/appsettings.Production.json` - Production config
2. ✅ `bigjobhunterpro-web/.env.production` - Frontend prod env vars
3. ✅ `bigjobhunterpro-web/public/staticwebapp.config.json` - SPA routing config
4. ✅ `.github/workflows/deploy-backend.yml` - CI/CD pipeline

### Files to Modify:

1. ✅ `src/WebAPI/appsettings.json`:line:12 - Remove hardcoded API key
2. ✅ `src/WebAPI/Program.cs`:18-29 - Add SQL retry logic
3. ✅ `src/WebAPI/Program.cs`:112-122 - Update CORS for production
4. ✅ `src/WebAPI/Program.cs`:82 - Add Application Insights (optional)
5. ✅ `src/WebAPI/WebAPI.csproj` - Add App Insights package (optional)

---

## Cost Breakdown (Monthly)

| Resource             | Tier              | Cost        |
| -------------------- | ----------------- | ----------- |
| Azure SQL Database   | Basic (Free tier) | $0          |
| App Service Plan     | B1                | ~$13        |
| Static Web App       | Free              | $0          |
| Key Vault            | Standard          | ~$0.03      |
| Application Insights | Pay-as-you-go     | ~$2-5       |
| **TOTAL**            |                   | **~$15-18** |

**Cost Optimization**: For testing/low traffic, use F1 (Free) App Service tier instead of B1 to reduce to $0-5/month.

---

## Troubleshooting Guide

### Issue: App Service shows "Service Unavailable"

**Solution**: Check App Service logs for startup errors, verify Key Vault access

### Issue: Frontend can't reach API (CORS error)

**Solution**: Verify `AllowedOrigins__0` matches Static Web App URL exactly

### Issue: Database connection fails

**Solution**: Check SQL Server firewall rules include App Service outbound IPs

### Issue: Key Vault secrets not loading

**Solution**: Verify Managed Identity is assigned and has "Get" permission on secrets

### Issue: JWT authentication fails

**Solution**: Check that JWT secret is properly loaded from Key Vault (not placeholder)

---

## Next Steps After Deployment

### Immediate (Week 1):

- [ ] Custom domain setup (e.g., bigjobhunter.pro)
- [ ] SSL certificate for custom domain
- [ ] Update JWT Issuer/Audience to match domain
- [ ] Rotate secrets (generate new JWT secret for production)

### Short-term (Month 1):

- [ ] Set up automated backups for Azure SQL
- [ ] Configure deployment slots for zero-downtime deployments
- [ ] Add health check monitoring alerts
- [ ] Implement rate limiting on API

### Medium-term (Quarter 1):

- [ ] Auto-scaling rules for App Service
- [ ] Redis cache for JWT validation
- [ ] Email service integration (SendGrid/Azure Communication Services)
- [ ] Implement remaining features (Hunting Parties, Leaderboards)

---

## Security Checklist

- [x] No secrets in source control
- [x] Secrets stored in Azure Key Vault
- [x] Managed Identity for secret access (no connection strings)
- [x] HTTPS enforced on all services
- [x] CORS restricted to known origins
- [x] SQL injection protection (EF Core parameterized queries)
- [x] Password policy enforced (8+ chars, uppercase, digit)
- [x] JWT tokens with expiration (7 days)
- [x] Account lockout after 5 failed attempts
- [ ] Regular dependency updates (Dependabot)
- [ ] Regular secret rotation (quarterly)

---

## Quick Reference URLs (After Deployment)

- **Backend API**: https://bjhp-api-prod.azurewebsites.net
- **Frontend App**: https://bjhp-frontend-prod.azurestaticapps.net
- **Health Check**: https://bjhp-api-prod.azurewebsites.net/api/health
- **Database**: bjhp-dev-sql-cgta.database.windows.net
- **Key Vault**: bjhp-keyvault-prod

---

**Estimated Total Time**: 2-3 hours
**Difficulty**: Intermediate
**Prerequisites**: Azure subscription, Azure CLI installed, GitHub account with repo access
