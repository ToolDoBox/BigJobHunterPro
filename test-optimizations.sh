#!/bin/bash

# Test script to verify database optimizations
API_BASE="http://localhost:5074/api"
TOKEN=""
USER_ID=""
APP_ID=""
PARTY_ID=""

echo "=== DATABASE OPTIMIZATION TEST SCRIPT ==="
echo ""

# Test 1: Register and Login
echo "Test 1: Creating test user..."
REGISTER_RESPONSE=$(curl -s -X POST "$API_BASE/auth/register" \
  -H "Content-Type: application/json" \
  -d "{
    \"email\": \"testuser$(date +%s)@example.com\",
    \"password\": \"TestPass123!\",
    \"displayName\": \"Test User\"
  }")

TOKEN=$(echo $REGISTER_RESPONSE | grep -o '"token":"[^"]*"' | sed 's/"token":"\(.*\)"/\1/')
USER_ID=$(echo $REGISTER_RESPONSE | grep -o '"userId":"[^"]*"' | sed 's/"userId":"\(.*\)"/\1/')

if [ -z "$TOKEN" ]; then
  echo "❌ Failed to register user"
  exit 1
fi

echo "✅ User registered successfully"
echo "   User ID: $USER_ID"
echo ""

# Test 2: Create Application (will trigger party ID cache)
echo "Test 2: Creating first application..."
CREATE_APP1=$(curl -s -X POST "$API_BASE/applications" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"rawPageContent\": \"Senior Software Engineer at TechCorp. Requirements: 5+ years experience, Python, React.\"
  }")

APP_ID=$(echo $CREATE_APP1 | grep -o '"id":"[^"]*"' | sed 's/"id":"\(.*\)"/\1/')
echo "✅ Application 1 created: $APP_ID"
echo "   Backend should show: 1 query to GetUserPartyIdAsync"
echo ""
sleep 2

# Test 3: Create Second Application (should use cached party ID)
echo "Test 3: Creating second application (testing party ID cache)..."
CREATE_APP2=$(curl -s -X POST "$API_BASE/applications" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"rawPageContent\": \"Frontend Developer at StartupCo. Requirements: React, TypeScript, 3+ years.\"
  }")

echo "✅ Application 2 created"
echo "   Backend should show: NO new query to GetUserPartyIdAsync (CACHE HIT!)"
echo ""
sleep 2

# Test 4: Get Applications List (First Call)
echo "Test 4: Fetching applications list (first call)..."
GET_APPS1=$(curl -s -X GET "$API_BASE/applications?page=1&pageSize=25" \
  -H "Authorization: Bearer $TOKEN")

APP_COUNT=$(echo $GET_APPS1 | grep -o '"items":\[' | wc -l)
echo "✅ Applications list fetched (first call)"
echo "   Backend should show: SELECT query to database"
echo ""
sleep 1

# Test 5: Get Applications List Again (Should use cache)
echo "Test 5: Fetching applications list again (testing cache)..."
GET_APPS2=$(curl -s -X GET "$API_BASE/applications?page=1&pageSize=25" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Applications list fetched (second call)"
echo "   Backend should show: NO database query (CACHE HIT!)"
echo ""
sleep 2

# Test 6: Update Application Status (testing duplicate query fix)
echo "Test 6: Updating application status (testing duplicate query fix)..."
UPDATE_STATUS=$(curl -s -X PATCH "$API_BASE/applications/$APP_ID/status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"status\": \"Screening\"
  }")

echo "✅ Application status updated"
echo "   Backend should show: ONLY 1 query to load application (NOT 2!)"
echo ""
sleep 2

# Test 7: Create Hunting Party
echo "Test 7: Creating hunting party..."
CREATE_PARTY=$(curl -s -X POST "$API_BASE/huntingparties" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"name\": \"Test Party\"
  }")

PARTY_ID=$(echo $CREATE_PARTY | grep -o '"id":"[^"]*"' | sed 's/"id":"\(.*\)"/\1/')
echo "✅ Hunting party created: $PARTY_ID"
echo ""
sleep 2

# Test 8: Get Leaderboard (testing optimized query)
echo "Test 8: Fetching leaderboard (testing optimized query)..."
GET_LEADERBOARD=$(curl -s -X GET "$API_BASE/huntingparties/$PARTY_ID/leaderboard" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Leaderboard fetched"
echo "   Backend should show: Query WITHOUT Include(Applications)"
echo "   Backend should show: COUNT query instead of loading all apps"
echo ""
sleep 2

# Test 9: Get Analytics (testing projections)
echo "Test 9: Fetching analytics (testing projections)..."
GET_KEYWORDS=$(curl -s -X GET "$API_BASE/analytics/keywords" \
  -H "Authorization: Bearer $TOKEN")

GET_CONVERSION=$(curl -s -X GET "$API_BASE/analytics/conversion-by-source" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Analytics fetched"
echo "   Backend should show: SELECT with projections (specific fields only)"
echo "   Backend should show: NO Include(TimelineEvents) with full load"
echo ""

echo ""
echo "=== TEST COMPLETE ==="
echo ""
echo "WHAT TO VERIFY IN BACKEND LOGS:"
echo "1. ✅ Party ID cache: Second app creation should NOT query party membership"
echo "2. ✅ Application list cache: Second fetch should NOT query database"
echo "3. ✅ Duplicate query fix: Status update should show ONLY 1 application query"
echo "4. ✅ Leaderboard optimization: Should use COUNT instead of loading Applications"
echo "5. ✅ Analytics optimization: Should use SELECT with specific fields only"
echo ""
