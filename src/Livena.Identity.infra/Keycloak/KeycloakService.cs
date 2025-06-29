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
            id = user.Id,
            username = user.Username,
            email = user.Email ?? $"{user.Username}@placeholder.livena",
            enabled = true,
            emailVerified = user.IsVerified ?? false,
            attributes = new
            {
                phone = user.Phone,
                birthday = user.Birthday.ToString("yyyy-MM-dd")
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
        response.EnsureSuccessStatusCode();
    }
    
    public async Task Update(string keycloakUserId, object input, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/admin/realms/{_realm}/users/{keycloakUserId}")
        {
            Content = CreateJsonContent(input)
        };

        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task Delete(string keycloakUserId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{_realm}/users/{keycloakUserId}");
        await AddAuthorizationAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
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
        response.EnsureSuccessStatusCode();

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
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        return data.GetProperty("access_token").GetString()!;
    }
}