<center>
  <p align="center">
    <img src="https://user-images.githubusercontent.com/20674439/158480674-3b8895e7-420e-4025-bd78-8058ba255476.png"  width="150" />
  </p>  
  <h1 align="center">🚀 Livina Identity Microservice with .NET</h1>
  <p align="center">
    Using Clean Architecture, DDD and the main current market best practices
  </p>
</center>
<br />

## How to run?

- Simply clone the Repository:

```sh
git clone https://github.com/herlanderbento/livena-identity.git
```

- Then run the solution file with Rider

<br />

## Tools needed

- Rider or Visual Studio 2022
- .NET 8 SDK installed
- Docker or Docker Desktop

## Running with Docker

To start the application and database using Docker, use the following command:

```sh
docker-compose up -d
```

This will spin up the necessary containers for the application and database.

## Keycloak Setup (Required after every Keycloak deployment)

⚠️ **IMPORTANT:** Ensure the `backend-client` has the correct permissions assigned to its Service Account:

1. Go to **Clients** → `backend-client` → **Service Account Roles**
2. Select the client: `realm-management`
3. Add the following roles:
   - `manage-users`
   - `view-users`
4. *(Optional)*: You may also add roles such as `query-users`, `view-realm`, `create-client`, etc., if needed for advanced scenarios.

5. In the **Authentication** settings, under **Required Actions**, make sure to **disable all required actions** (none should be enabled).

> **Note:** You must repeat this configuration every time you redeploy or reset your Keycloak instance.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
