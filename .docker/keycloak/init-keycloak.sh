#!/bin/sh

set -e

KEYCLOAK_URL="http://localhost:8081"
KEYCLOAK_INTERNAL="http://localhost:8080"
REALM="livena-dev"
CLIENT_ID="backend-client"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"

echo "⏳ Aguardando Keycloak ficar pronto..."
until curl -s "$KEYCLOAK_URL/health" > /dev/null; do
  echo "🔄 Esperando Keycloak..."
  sleep 5
done
echo "✅ Keycloak pronto!"

echo "🔐 Obtendo token de admin..."
ADMIN_TOKEN=$(curl -s -X POST "$KEYCLOAK_URL/realms/master/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" \
  -d "username=$ADMIN_USER" \
  -d "password=$ADMIN_PASSWORD" | jq -r '.access_token')

[ -z "$ADMIN_TOKEN" ] && echo "❌ Erro ao obter token." && exit 1
echo "✅ Token obtido."

get_client_id() {
  curl -s -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" \
    "$KEYCLOAK_URL/admin/realms/$REALM/clients" | jq -r ".[] | select(.clientId == \"$1\") | .id"
}

get_role_id() {
  curl -s -H "Authorization: Bearer $ADMIN_TOKEN" -H "Content-Type: application/json" \
    "$KEYCLOAK_URL/admin/realms/$REALM/clients/$1/roles" | jq -r ".[] | select(.name == \"$2\") | .id"
}

BACKEND_CLIENT_ID=$(get_client_id "$CLIENT_ID")
REALM_MANAGEMENT_ID=$(get_client_id "realm-management")
MANAGE_USERS_ROLE_ID=$(get_role_id "$REALM_MANAGEMENT_ID" "manage-users")
VIEW_USERS_ROLE_ID=$(get_role_id "$REALM_MANAGEMENT_ID" "view-users")

[ -z "$BACKEND_CLIENT_ID" ] && echo "❌ backend-client não encontrado." && exit 1
[ -z "$REALM_MANAGEMENT_ID" ] && echo "❌ realm-management não encontrado." && exit 1
[ -z "$MANAGE_USERS_ROLE_ID" ] && echo "❌ manage-users role não encontrada." && exit 1
[ -z "$VIEW_USERS_ROLE_ID" ] && echo "❌ view-users role não encontrada." && exit 1

echo "✅ IDs obtidos."

echo "🔎 Buscando service account ID..."
SERVICE_ACCOUNT_ID=$(curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
  "$KEYCLOAK_URL/admin/realms/$REALM/clients/$BACKEND_CLIENT_ID/service-account-user" | jq -r '.id')

[ -z "$SERVICE_ACCOUNT_ID" ] && echo "❌ Service account não encontrada." && exit 1

echo "✅ Service account ID: $SERVICE_ACCOUNT_ID"

echo "➕ Atribuindo roles à service account..."

curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/users/$SERVICE_ACCOUNT_ID/role-mappings/clients/$REALM_MANAGEMENT_ID" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "[
    {\"id\": \"$MANAGE_USERS_ROLE_ID\", \"name\": \"manage-users\"},
    {\"id\": \"$VIEW_USERS_ROLE_ID\", \"name\": \"view-users\"}
  ]"

echo "✅ Roles atribuídas à service account."

echo "🌐 Atribuindo role 'user' como padrão do realm..."

# Autenticando via kcadm dentro do container
/opt/keycloak/bin/kcadm.sh config credentials \
  --server "$KEYCLOAK_INTERNAL" \
  --realm master \
  --user "$ADMIN_USER" \
  --password "$ADMIN_PASSWORD"

# Adicionando a role 'user' como default
/opt/keycloak/bin/kcadm.sh add-roles \
  --rname default-roles-$REALM \
  --rolename user \
  --realm $REALM

echo "✅ Role 'user' adicionada como default do realm $REALM."
