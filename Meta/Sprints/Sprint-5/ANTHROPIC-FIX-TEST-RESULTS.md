# Anthropic API Key Issue - Local Test Results

## Problem Summary
Production API key has never been used despite being correctly configured in Azure Key Vault with proper permissions.

## Root Cause
**Environment variable name mismatch**: The env var `AnthropicApiKey` doesn't override the nested configuration path `AnthropicSettings:ApiKey` in `appsettings.Production.json`.

## Test Results (Local)

### ❌ Test 1: Current Production Setup (WRONG)
```bash
ASPNETCORE_ENVIRONMENT=Production
AnthropicApiKey="sk-ant-test-WRONG-VAR"
```

**Output:**
```
warn: Infrastructure.Services.AiParsingService[0]
      Anthropic API key is not configured. State=placeholder. AI parsing will be skipped.
```

**Result:** App ignores the env var and reads placeholder from appsettings.Production.json

---

### ✅ Test 2: Proposed Fix (CORRECT)
```bash
ASPNETCORE_ENVIRONMENT=Production
AnthropicSettings__ApiKey="sk-ant-test-CORRECT-VAR"
```

**Output:**
```
(No Anthropic warnings - the key was accepted)
```

**Result:** App successfully reads the environment variable!

---

## Azure Investigation Findings

### ✅ Key Vault Configuration
- Secret exists: `AnthropicApiKey`
- Secret value: `sk-ant-api03-g1i...` (valid API key)
- Secret is enabled: Yes
- Created: 2026-01-06

### ✅ Managed Identity Permissions
- Principal ID: `abded0d8-d62a-4272-9d20-3ba5408f0f3e`
- Key Vault access: `get` and `list` permissions on secrets
- Status: **Properly configured**

### ❌ Environment Variable (THE PROBLEM)
**Current:**
```
Name: AnthropicApiKey
Value: @Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)
```

**Should be:**
```
Name: AnthropicSettings:ApiKey  (or AnthropicSettings__ApiKey)
Value: @Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)
```

### Production Logs Evidence
From `eventlog.prev.xml`:
```
Anthropic API key is not configured. State=placeholder. AI parsing will be skipped.
```
(Repeated 20+ times on 2026-01-08)

---

## Solution

Update the Azure App Service environment variable name from `AnthropicApiKey` to `AnthropicSettings:ApiKey` to match the configuration path.

### Azure CLI Command
```bash
# Remove old variable
az webapp config appsettings delete \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG \
  --setting-names AnthropicApiKey

# Add correct variable
az webapp config appsettings set \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG \
  --settings "AnthropicSettings:ApiKey=@Microsoft.KeyVault(SecretUri=https://bjhp-keyvault-prod.vault.azure.net/secrets/AnthropicApiKey/)"

# Restart app service
az webapp restart \
  --name bjhp-api-prod \
  --resource-group BigJobHunterPro-Prod-RG
```

---

## Confidence Level
**🔴 HIGH** - Local testing confirms the exact same behavior observed in production logs.

## Date
2026-01-09
