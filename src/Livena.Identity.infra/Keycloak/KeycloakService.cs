using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Entity;

namespace Livena.Identity.infra.Keycloak;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _realm;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tokenUrl;

    private string? _adminToken;

    public KeycloakService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _realm = _configuration["Keycloak:Realm"]!;
        _clientId = _configuration["Keycloak:ClientId"]!;
        _clientSecret = _configuration["Keycloak:ClientSecret"]!;
        _tokenUrl = _configuration["Keycloak:TokenUrl"]!;
    }

    public async Task Insert(User user, string password, CancellationToken cancellationToken)
    {
        var keycloakUser = (new
        {
            username = user.Username,
            email = user.Email ?? $"{user.Username}@placeholder.livena",
            enabled = true,
            emailVerified = user.IsVerified ?? false,
            attributes = new
            {
                phone = user.Phone,
                birthday = user.Birthday.ToString("yyyy-MM-dd"),
                externalId = user.Id.ToString()
            },
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
        });
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users")
        {
            Content = CreateJsonContent(keycloakUser)
        };

        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to create user in Keycloak. Status: {response.StatusCode}. Response: {errorContent}");
        }

        // Wait a moment for the user to be fully created
        await Task.Delay(1000, cancellationToken);
        
        // Find the user by username and assign user role directly
        var keycloakUserId = await GetKeycloakIdByUsername(user.Username, cancellationToken);
        if (!string.IsNullOrEmpty(keycloakUserId))
        {
            await AssignUserRoleDirectly(keycloakUserId, cancellationToken);
        }
    }

    private async Task AssignUserRoleDirectly(string keycloakUserId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Starting to assign user role for user ID: {keycloakUserId}");
            
            // Use the known role ID directly (we know it from our previous tests)
            string roleId = "1c8cbc44-e301-4cb2-9dee-bffa9b295c50";
            Console.WriteLine($"Using role ID: {roleId}");

            // Assign the role directly to the user (this will appear in realm_access.roles)
            var assignRoleRequest = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users/{keycloakUserId}/role-mappings/realm")
            {
                Content = CreateJsonContent(new[] { new { id = roleId, name = "user" } })
            };
            
            var requestBody = await assignRoleRequest.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Assign role request body: {requestBody}");
            
            await AddAuthorizationAsync(assignRoleRequest, cancellationToken);
            var assignResponse = await _httpClient.SendAsync(assignRoleRequest, cancellationToken);
            
            Console.WriteLine($"Assign role response status: {assignResponse.StatusCode}");
            
            if (!assignResponse.IsSuccessStatusCode)
            {
                var errorContent = await assignResponse.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Warning: Failed to assign user role directly. Status: {assignResponse.StatusCode}. Response: {errorContent}");
            }
            else
            {
                Console.WriteLine("Successfully assigned user role directly!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error assigning user role directly: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public async Task<string?> GetKeycloakIdByExternalId(string externalId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Looking for user with externalId: {externalId}");
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?exact=true&first=0&max=1&q=externalId:{externalId}");
        Console.WriteLine($"Search URL: {request.RequestUri}");
        
        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        Console.WriteLine($"Search response status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Failed to search user. Status: {response.StatusCode}. Response: {errorContent}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine($"Search response: {json}");
        
        var users = JsonSerializer.Deserialize<JsonElement>(json);

        if (users.ValueKind == JsonValueKind.Array && users.GetArrayLength() > 0)
        {
            var userId = users[0].GetProperty("id").GetString();
            Console.WriteLine($"Found user ID: {userId}");
            return userId;
        }
        
        Console.WriteLine("No user found with this externalId");
        return null;
    }

    public async Task<string?> GetKeycloakIdByUsername(string username, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Looking for user with username: {username}");
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users?search={username}");
        Console.WriteLine($"Search URL: {request.RequestUri}");
        
        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        Console.WriteLine($"Search response status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Failed to search user. Status: {response.StatusCode}. Response: {errorContent}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine($"Search response: {json}");
        
        var users = JsonSerializer.Deserialize<JsonElement>(json);

        if (users.ValueKind == JsonValueKind.Array && users.GetArrayLength() > 0)
        {
            var userId = users[0].GetProperty("id").GetString();
            Console.WriteLine($"Found user ID: {userId}");
            return userId;
        }
        
        Console.WriteLine("No user found with this username");
        return null;
    }

    public async Task Update(User user, CancellationToken cancellationToken)
    {
        var keycloakUserId = await GetKeycloakIdByExternalId(user.Id.ToString(), cancellationToken);
        if (keycloakUserId != null)
        {
            var keycloakUpdate = new
            {
                email = user.Email ?? $"{user.Username}@placeholder.livena",
                emailVerified = user.IsVerified ?? false,
                attributes = new
                {
                    phone = user.Phone,
                    birthday = user.Birthday.ToString("yyyy-MM-dd"),
                    externalId = user.Id.ToString()
                }
            };
            
            var request = new HttpRequestMessage(HttpMethod.Put, $"/admin/realms/{_realm}/users/{keycloakUserId}")
            {
                Content = CreateJsonContent(keycloakUpdate)
            };

            await AddAuthorizationAsync(request, cancellationToken);
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task Delete(string keycloakUserId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{keycloakUserId}");
        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<object?> GetById(string keycloakUserId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/{_realm}/users/{keycloakUserId}");
        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    public async Task<KeycloakTokenResponse> Login(string username, string password, CancellationToken cancellationToken)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

        var response = await _httpClient.PostAsync(_tokenUrl, content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Keycloak authentication failed with status {response.StatusCode}. Response: {errorContent}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        return new KeycloakTokenResponse
        {
            AccessToken = data.GetProperty("access_token").GetString()!,
            RefreshToken = data.GetProperty("refresh_token").GetString()!,
            ExpiresIn = data.GetProperty("expires_in").GetInt32()
        };
    }

    private static StringContent CreateJsonContent(object obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task AddAuthorizationAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_adminToken))
        {
            _adminToken = await GetAdminToken(cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
    }

    private async Task<string> GetAdminToken(CancellationToken cancellationToken)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        });

        var response = await _httpClient.PostAsync(_tokenUrl, content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to get admin token from Keycloak. Status: {response.StatusCode}. Response: {errorContent}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        return data.GetProperty("access_token").GetString()!;
    }
}