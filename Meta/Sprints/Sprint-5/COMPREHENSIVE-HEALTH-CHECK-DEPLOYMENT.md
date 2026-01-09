# Comprehensive Health Check - Deployment Guide

## Overview

A comprehensive health check system has been implemented to accurately detect when the backend API is fully ready to handle requests. This fixes the issue where the status light showed "online" even when the backend was not fully initialized.

## What Changed

### Backend Changes

1. **New Health Check Service** (`src/Infrastructure/Services/HealthCheckService.cs`)
   - Validates database connectivity with timeout
   - Checks JWT configuration
   - Verifies database migrations are applied
   - Validates critical configuration settings
   - Returns detailed component health status

2. **New Endpoint** (`/api/health/ready`)
   - Returns HTTP 200 OK only when ALL critical components are healthy
   - Returns HTTP 503 Service Unavailable if any component is degraded or unhealthy
   - Provides detailed breakdown of each component's health

3. **Service Registration** (`src/WebAPI/Program.cs:160`)
   - Registered `IHealthCheckService` in dependency injection

### Frontend Changes

1. **Updated Status Check** (`bigjobhunterpro-web/src/hooks/useServerStatus.ts`)
   - Now uses `/api/health/ready` instead of `/api/health`
   - Increased timeout to 5 seconds (from 2.5s) for comprehensive check
   - Only shows "online" when backend returns 200 OK (fully healthy)

## Azure App Service Configuration

### Update Health Check Path

After deploying these changes, update your Azure App Service to use the new readiness endpoint:

#### Option 1: Azure Portal (Recommended)

1. Navigate to: **Azure Portal** → **App Service (bjhp-api-prod)** → **Configuration** → **General settings**
2. Update **Health check path** from `/api/health` to `/api/health/ready`
3. Click **Save** at the top

#### Option 2: Azure CLI

```bash
az webapp config set \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG \
  --generic-configurations '{"healthCheckPath": "/api/health/ready"}'
```

### Health Check Behavior

**Before (Old `/api/health` endpoint):**
- Always returned 200 OK immediately
- Did not validate any dependencies
- Could show "online" during cold starts before app was ready

**After (New `/api/health/ready` endpoint):**
- Returns 200 OK only when all components are healthy:
  - ✅ Database connected and responsive
  - ✅ JWT configuration valid
  - ✅ All migrations applied
  - ✅ Critical config present
- Returns 503 Service Unavailable if any component fails
- Provides detailed diagnostics in response body

## Testing the Health Check

### Local Testing

```bash
# Start the backend
cd src/WebAPI
dotnet run

# Test the readiness endpoint
curl http://localhost:5001/api/health/ready | jq
```

**Expected Response (Healthy):**
```json
{
  "status": "Healthy",
  "timestampUtc": "2026-01-09T12:00:00Z",
  "components": {
    "Database": {
      "status": "Healthy",
      "description": "Database is connected and responsive",
      "duration": "00:00:00.0234567",
      "data": {
        "provider": "Microsoft.EntityFrameworkCore.SqlServer",
        "userCount": 5
      }
    },
    "JWT": {
      "status": "Healthy",
      "description": "JWT authentication is properly configured",
      "duration": "00:00:00.0012345",
      "data": {
        "issuer": "BigJobHunterPro",
        "audience": "BigJobHunterPro"
      }
    },
    "Migrations": {
      "status": "Healthy",
      "description": "Database schema is up to date",
      "duration": "00:00:00.0456789",
      "data": {
        "appliedCount": 2
      }
    },
    "Configuration": {
      "status": "Healthy",
      "description": "All critical configuration is present",
      "duration": "00:00:00.0001234",
      "data": {
        "environment": "Development"
      }
    }
  },
  "messages": [],
  "duration": "00:00:00.0705935"
}
```

**Expected Response (Unhealthy - Database Down):**
```json
HTTP 503 Service Unavailable

{
  "status": "Unhealthy",
  "timestampUtc": "2026-01-09T12:00:00Z",
  "components": {
    "Database": {
      "status": "Unhealthy",
      "description": "Cannot connect to database",
      "error": "Database connection failed",
      "duration": "00:00:05.0000000"
    },
    "JWT": {
      "status": "Healthy",
      "description": "JWT authentication is properly configured",
      "duration": "00:00:00.0012345"
    },
    ...
  },
  "messages": [],
  "duration": "00:00:05.0123456"
}
```

### Production Testing

After deployment, test the production endpoint:

```bash
# Test readiness
curl https://bjhp-api-prod.azurewebsites.net/api/health/ready

# Should return 200 OK with detailed health status
# If returns 503, check the response body for which component is unhealthy
```

### Frontend Testing

1. Open your frontend (login or register page)
2. Watch the status indicator below the logo
3. The light should only show **green (Server online)** when:
   - Backend responds with 200 OK
   - All critical components are healthy
4. The light should show **red (Server offline)** when:
   - Backend is unreachable
   - Backend returns 503 (degraded/unhealthy)
   - Request times out (>5 seconds)

## Deployment Steps

### 1. Build and Test Locally

```bash
# Build the solution
dotnet build BigJobHunterPro.sln

# Run tests (if applicable)
dotnet test

# Start the API locally
cd src/WebAPI
dotnet run

# In another terminal, test the endpoint
curl http://localhost:5001/api/health/ready | jq
```

### 2. Commit and Push

```bash
git add .
git commit -m "feat: Add comprehensive health check system

- Add HealthCheckService to validate all critical components
- Add /api/health/ready endpoint for readiness checks
- Update frontend to use readiness endpoint
- Improve cold start detection and status accuracy"

git push origin main
```

### 3. Monitor Deployment

1. Watch GitHub Actions workflow complete
2. Wait for Azure App Service to restart
3. Test the production endpoint

### 4. Update Azure Health Check Path

Follow the steps in the "Azure App Service Configuration" section above.

### 5. Verify Frontend

1. Navigate to https://bjhp-frontend-prod.azurestaticapps.net/login
2. Observe the status light
3. Verify it shows accurate status

## Troubleshooting

### Status Light Shows Offline But API Works

**Possible Causes:**
1. Health check timeout is too short
2. One component is degraded (migrations pending, config warning)
3. Database query is slow

**Solution:**
- Check the `/api/health/ready` response in browser DevTools
- Review which component is unhealthy
- Address the specific component issue

### Health Check Times Out

**Possible Causes:**
1. Database is slow to respond
2. Cold start in progress
3. Network issues

**Solution:**
- Health check has built-in 5-second timeouts for database operations
- Frontend timeout is 5 seconds
- If consistently timing out, investigate database performance

### Shows Healthy But Login Fails

**This should no longer happen** because the readiness check validates:
- Database connectivity
- JWT configuration
- All migrations applied

If it does happen, it indicates a new dependency not covered by the health check.

## Monitoring

### Azure Application Insights

The health check logs warnings when components are unhealthy:

```
Health check failed: Unhealthy. Unhealthy components: Database, JWT
```

Set up alerts in Application Insights for these warnings.

### Azure Monitor

Create an alert rule for health check failures:

```bash
az monitor metrics alert create \
  --name "Health Check Failures" \
  --resource-group BigJobHunterPro-Prod-RG \
  --scopes /subscriptions/<sub-id>/resourceGroups/BigJobHunterPro-Prod-RG/providers/Microsoft.Web/sites/bjhp-api-prod \
  --condition "count HealthCheckStatus where ResultCode == 503 > 5" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --description "Alert when health checks fail more than 5 times in 5 minutes"
```

## Benefits

### User Experience
- ✅ Accurate status indicator on login/register pages
- ✅ No more false "online" status during cold starts
- ✅ Clear feedback when backend is initializing (~30s on first request)

### Operational
- ✅ Detailed diagnostics of component health
- ✅ Early detection of configuration issues
- ✅ Better visibility into system state
- ✅ Azure App Service uses it for health monitoring

### Development
- ✅ Easy to test individual component health
- ✅ Validates all critical dependencies
- ✅ Helps identify issues during deployment
- ✅ Extensible for future components

## Future Enhancements

Consider adding additional health checks for:
- [ ] Anthropic API connectivity
- [ ] SignalR hub status
- [ ] Cache/Redis connectivity (when added)
- [ ] Email service (when added)
- [ ] Storage account connectivity (when added)

## Related Files

### Backend
- `src/Application/Interfaces/IHealthCheckService.cs` - Service interface
- `src/Application/DTOs/Health/HealthCheckResult.cs` - Response DTOs
- `src/Infrastructure/Services/HealthCheckService.cs` - Implementation
- `src/WebAPI/Controllers/HealthController.cs` - Controller endpoints
- `src/WebAPI/Program.cs` - Service registration

### Frontend
- `bigjobhunterpro-web/src/hooks/useServerStatus.ts` - Status check hook
- `bigjobhunterpro-web/src/components/ui/ServerStatusIndicator.tsx` - UI component
- `bigjobhunterpro-web/src/pages/Login.tsx` - Uses status indicator
- `bigjobhunterpro-web/src/pages/Register.tsx` - Uses status indicator

## References

- Azure App Service Health Check: https://learn.microsoft.com/en-us/azure/app-service/monitor-instances-health-check
- ASP.NET Core Health Checks: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
