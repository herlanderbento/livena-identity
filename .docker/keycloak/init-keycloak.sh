#!/bin/bash

# Wait for Keycloak to be ready
echo "Waiting for Keycloak to be ready..."
until curl -s http://localhost:8081/health > /dev/null; do
    echo "Keycloak is not ready yet. Waiting..."
    sleep 5
done

echo "Keycloak is ready!"

# Get admin token
echo "Getting admin token..."
ADMIN_TOKEN=$(curl -s -X POST http://localhost:8081/realms/master/protocol/openid-connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "grant_type=password&client_id=admin-cli&username=admin&password=admin" | jq -r '.access_token')

if [ "$ADMIN_TOKEN" = "null" ] || [ -z "$ADMIN_TOKEN" ]; then
    echo "Failed to get admin token"
    exit 1
fi

echo "Admin token obtained successfully"

# Get backend-client ID
echo "Getting backend-client ID..."
BACKEND_CLIENT_ID=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.[] | select(.clientId == "backend-client") | .id')

if [ "$BACKEND_CLIENT_ID" = "null" ] || [ -z "$BACKEND_CLIENT_ID" ]; then
    echo "Failed to get backend-client ID"
    exit 1
fi

echo "Backend-client ID: $BACKEND_CLIENT_ID"

# Get realm-management client ID
echo "Getting realm-management client ID..."
REALM_MANAGEMENT_ID=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.[] | select(.clientId == "realm-management") | .id')

if [ "$REALM_MANAGEMENT_ID" = "null" ] || [ -z "$REALM_MANAGEMENT_ID" ]; then
    echo "Failed to get realm-management client ID"
    exit 1
fi

echo "Realm-management ID: $REALM_MANAGEMENT_ID"

# Get manage-users role ID
echo "Getting manage-users role ID..."
MANAGE_USERS_ROLE_ID=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients/$REALM_MANAGEMENT_ID/roles" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.[] | select(.name == "manage-users") | .id')

if [ "$MANAGE_USERS_ROLE_ID" = "null" ] || [ -z "$MANAGE_USERS_ROLE_ID" ]; then
    echo "Failed to get manage-users role ID"
    exit 1
fi

echo "Manage-users role ID: $MANAGE_USERS_ROLE_ID"

# Get view-users role ID
echo "Getting view-users role ID..."
VIEW_USERS_ROLE_ID=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients/$REALM_MANAGEMENT_ID/roles" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.[] | select(.name == "view-users") | .id')

if [ "$VIEW_USERS_ROLE_ID" = "null" ] || [ -z "$VIEW_USERS_ROLE_ID" ]; then
    echo "Failed to get view-users role ID"
    exit 1
fi

echo "View-users role ID: $VIEW_USERS_ROLE_ID"

# Get service account user ID
echo "Getting service account user ID..."
SERVICE_ACCOUNT_ID=$(curl -s -X GET "http://localhost:8081/admin/realms/livena-dev/clients/$BACKEND_CLIENT_ID/service-account-user" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" | jq -r '.id')

if [ "$SERVICE_ACCOUNT_ID" = "null" ] || [ -z "$SERVICE_ACCOUNT_ID" ]; then
    echo "Failed to get service account user ID"
    exit 1
fi

echo "Service account user ID: $SERVICE_ACCOUNT_ID"

# Assign manage-users and view-users roles to service account
echo "Assigning manage-users and view-users roles to service account..."
curl -s -X POST "http://localhost:8081/admin/realms/livena-dev/users/$SERVICE_ACCOUNT_ID/role-mappings/clients/$REALM_MANAGEMENT_ID" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" \
    -d "[{\"id\": \"$MANAGE_USERS_ROLE_ID\", \"name\": \"manage-users\"}, {\"id\": \"$VIEW_USERS_ROLE_ID\", \"name\": \"view-users\"}]"

if [ $? -eq 0 ]; then
    echo "Successfully assigned manage-users and view-users roles to service account"
else
    echo "Failed to assign roles to service account"
    exit 1
fi

echo "Keycloak permissions configured successfully!" 