# Anthropic API Key Fix - Deployment Summary

## ✅ Fix Deployed to Production

**Date:** 2026-01-09
**Time:** ~16:22 UTC
**Status:** COMPLETED

---

## Changes Applied

### 1. Environment Variable Renamed
**Before:**
```
Name: AnthropicApiKey
Value: @Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)
```

**After:**
```
Name: AnthropicSettings:ApiKey
Value: @Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)
```

### 2. App Service Restarted
The `bjhp-api-prod` App Service was restarted to apply the new configuration.

### 3. Configuration Verified
- ✅ Old environment variable removed
- ✅ New environment variable added with correct Key Vault reference
- ✅ App Service is running (health check returned 200 OK)

---

## Expected Outcome

### Before Fix (Old Behavior)
Application startup logs showed:
```
warn: Infrastructure.Services.AiParsingService[0]
      Anthropic API key is not configured. State=placeholder. AI parsing will be skipped.
```

### After Fix (Expected Behavior)
- ✅ No warning about "Anthropic API key is not configured"
- ✅ Application reads the Key Vault secret successfully
- ✅ AI parsing will be attempted when users submit applications
- ✅ Anthropic API key usage will appear in Anthropic dashboard

---

## Verification Steps

### Immediate Verification (Next 24 hours)

1. **Check Anthropic Dashboard**
   - Go to: https://console.anthropic.com/
   - Navigate to API Keys
   - Look for the key: `sk-ant-api03-g1i...mAAA`
   - **Expected:** "Last used at" should show a recent timestamp (not "Never")

2. **Test AI Parsing Feature**
   - Log into production: https://bigjobhunter.pro
   - Create a new job application
   - Paste job posting content
   - Submit the application
   - **Expected:** AI should extract company name, role, etc.

3. **Check Application Logs** (if needed)
   ```bash
   az webapp log download \
     --name bjhp-api-prod \
     --resource-group BigJobHunterPro-Prod-RG \
     --log-file new-logs.zip

   # Extract and search for recent Anthropic logs
   unzip new-logs.zip -d new-logs
   grep -r "Calling Anthropic API" new-logs/
   ```

   **Expected:** Logs should show "Calling Anthropic API to parse job posting" when AI parsing is triggered

---

## Technical Details

### Root Cause
ASP.NET Core configuration hierarchy caused the app to read from `appsettings.Production.json` instead of the environment variable:

1. Application checks `AnthropicSettings:ApiKey` in configuration
2. Finds placeholder in `appsettings.Production.json`: `"** LOADED FROM AZURE KEY VAULT **"`
3. Environment variable `AnthropicApiKey` doesn't override because it's a different config path
4. App detects placeholder and skips AI parsing

### Solution
Renamed environment variable from flat `AnthropicApiKey` to nested `AnthropicSettings:ApiKey` to match the configuration path in the code.

### Local Testing
Confirmed the fix works locally:
- ❌ `AnthropicApiKey="test"` → `State=placeholder` (rejected)
- ✅ `AnthropicSettings__ApiKey="test"` → No warnings (accepted)

---

## Commands Used

```bash
# Add new environment variable
az webapp config appsettings set \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG \
  --settings "AnthropicSettings:ApiKey=@Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)"

# Remove old environment variable
az webapp config appsettings delete \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG \
  --setting-names AnthropicApiKey

# Restart App Service
az webapp restart \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG
```

---

## Next Steps

1. ✅ Wait 24 hours and check Anthropic dashboard for usage
2. ✅ Test AI parsing feature manually
3. If still not working, check Application Insights for new error messages

---

## Related Files
- Local test results: `ANTHROPIC-FIX-TEST-RESULTS.md`
- Code reference: `src/Infrastructure/Services/AiParsingService.cs` (lines 32-44)
- Config reference: `src/WebAPI/appsettings.Production.json` (lines 11-12)
