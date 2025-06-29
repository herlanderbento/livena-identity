#!/bin/bash

echo "Testing Keycloak configuration..."

# Test if Keycloak is running
echo "1. Checking if Keycloak is running..."
if curl -s http://localhost:8081/health > /dev/null; then
    echo "   ✓ Keycloak is running"
else
    echo "   ✗ Keycloak is not running on port 8081"
    exit 1
fi

# Test client credentials grant
echo "2. Testing client credentials grant..."
CLIENT_CREDENTIALS_RESPONSE=$(curl -s -X POST http://localhost:8081/realms/livena-dev/protocol/openid-connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=client_credentials&client_id=backend-client&client_secret=RoeM1m9lHff3M74SO5tIagAw3jfPWzwG")

if echo "$CLIENT_CREDENTIALS_RESPONSE" | jq -e '.access_token' > /dev/null; then
    echo "   ✓ Client credentials grant works"
    ADMIN_TOKEN=$(echo "$CLIENT_CREDENTIALS_RESPONSE" | jq -r '.access_token')
else
    echo "   ✗ Client credentials grant failed:"
    echo "$CLIENT_CREDENTIALS_RESPONSE" | jq '.'
    exit 1
fi

# Test password grant (this should fail if no users exist)
echo "3. Testing password grant..."
PASSWORD_RESPONSE=$(curl -s -X POST http://localhost:8081/realms/livena-dev/protocol/openid-connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password&client_id=backend-client&client_secret=RoeM1m9lHff3M74SO5tIagAw3jfPWzwG&username=testuser&password=testpass")

echo "   Password grant response:"
echo "$PASSWORD_RESPONSE" | jq '.'

# Check if backend-client supports password grant
echo "4. Checking backend-client configuration..."
CLIENT_CONFIG=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.[] | select(.clientId == "backend-client")')

echo "   Backend-client configuration:"
echo "$CLIENT_CONFIG" | jq '.'

echo "Test completed!" 