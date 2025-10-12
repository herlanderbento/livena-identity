using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Livena.Identity.infra.OAuth;

public class GoogleOAuthService : IOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IOAuthAccountRepository _oAuthAccountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GoogleOAuthService> _logger;
    private readonly string _clientId;
    private readonly string? _clientSecret;

    public GoogleOAuthService(
        HttpClient httpClient,
        IConfiguration configuration,
        IOAuthAccountRepository oAuthAccountRepository,
        IUserRepository userRepository,
        ILogger<GoogleOAuthService> logger
    )
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _oAuthAccountRepository = oAuthAccountRepository;
        _userRepository = userRepository;
        _logger = logger;

        _clientId = _configuration["GoogleOAuth:ClientId"]!;
        _clientSecret = _configuration["GoogleOAuth:ClientSecret"]!;
    }

    public async Task<OAuthUserInfo> GetUserInfo(
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken
    )
    {
        var tokenResponse = await ExchangeCodeForTokens(
            authorizationCode ?? string.Empty,
            redirectUri,
            cancellationToken
        );

        var userInfo = await GetGoogleUserInfo(
            tokenResponse.AccessToken ?? string.Empty,
            cancellationToken
        );

        string providerUserId = userInfo.Sub;

        if (string.IsNullOrEmpty(providerUserId) && !string.IsNullOrEmpty(tokenResponse.IdToken))
        {
            providerUserId = ExtractSubFromIdToken(tokenResponse.IdToken);
        }

        if (string.IsNullOrEmpty(providerUserId))
        {
            throw new System.ApplicationException(
                "Google user info Sub field is required but was null or empty"
            );
        }

        var birthday = await GetGoogleBirthday(
            tokenResponse.AccessToken ?? string.Empty,
            cancellationToken
        );

        var oAuthUserInfo = new OAuthUserInfo
        {
            Provider = "google",
            ProviderUserId = providerUserId,
            Email = userInfo.Email,
            FirstName = userInfo.GivenName,
            LastName = userInfo.FamilyName,
            Picture = userInfo.Picture,
            AccessToken = tokenResponse.AccessToken ?? string.Empty,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            Birthday = birthday,
        };

        return oAuthUserInfo;
    }

    public async Task<KeycloakTokenResponse> ExchangeCodeForTokens(
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken
    )
    {
        var tokenRequest = new
        {
            grant_type = "authorization_code",
            client_id = _clientId,
            client_secret = _clientSecret,
            code = authorizationCode,
            redirect_uri = redirectUri,
        };

        var content = new FormUrlEncodedContent(
            new[]
            {
                new KeyValuePair<string, string>("grant_type", tokenRequest.grant_type),
                new KeyValuePair<string, string>("client_id", tokenRequest.client_id),
                new KeyValuePair<string, string>(
                    "client_secret",
                    tokenRequest.client_secret ?? string.Empty
                ),
                new KeyValuePair<string, string>("code", tokenRequest.code ?? string.Empty),
                new KeyValuePair<string, string>("redirect_uri", tokenRequest.redirect_uri),
            }
        );

        var response = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            content,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Google OAuth token exchange failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode,
                errorContent
            );
        }

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

        var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(
            jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (tokenData == null)
        {
            _logger.LogError("Failed to deserialize Google token response");
            throw new System.ApplicationException("Failed to deserialize Google token response");
        }

        return new KeycloakTokenResponse
        {
            AccessToken = tokenData.AccessToken ?? string.Empty,
            RefreshToken = tokenData.RefreshToken ?? string.Empty,
            ExpiresIn = tokenData.ExpiresIn,
            IdToken = tokenData.IdToken,
        };
    }

    public async Task<User> FindUserByOAuthAccount(
        string provider,
        string providerUserId,
        CancellationToken cancellationToken
    )
    {
        var oAuthAccount = await _oAuthAccountRepository.GetByProviderAndProviderUserId(
            provider,
            providerUserId,
            cancellationToken
        );

        if (oAuthAccount == null)
        {
            throw new NotFoundException("OAuth account not found.");
        }

        var user = await _userRepository.GetById(oAuthAccount.UserId, cancellationToken);

        return user;
    }

    public async Task<OAuthAccount> CreateOAuthAccount(
        Guid userId,
        OAuthUserInfo userInfo,
        CancellationToken cancellationToken
    )
    {
        var oAuthAccount = new OAuthAccount(
            userId,
            userInfo.Provider,
            userInfo.ProviderUserId,
            userInfo.AccessToken,
            userInfo.RefreshToken,
            userInfo.ExpiresAt
        );

        await _oAuthAccountRepository.Insert(oAuthAccount, cancellationToken);

        return oAuthAccount;
    }

    public async Task<OAuthAccount?> GetOAuthAccount(
        Guid userId,
        string provider,
        CancellationToken cancellationToken
    )
    {
        var oAuthAccount = await _oAuthAccountRepository.GetByUserIdAndProvider(
            userId,
            provider,
            cancellationToken
        );

        return oAuthAccount;
    }

    public async Task UpdateOAuthAccount(
        OAuthAccount oAuthAccount,
        OAuthUserInfo userInfo,
        CancellationToken cancellationToken
    )
    {
        var updatedAccount = new OAuthAccount(
            oAuthAccount.UserId,
            oAuthAccount.Provider,
            oAuthAccount.ProviderUserId,
            userInfo.AccessToken,
            userInfo.RefreshToken,
            userInfo.ExpiresAt
        );

        await _oAuthAccountRepository.Update(updatedAccount, cancellationToken);
    }

    private async Task<GoogleUserInfo> GetGoogleUserInfo(
        string accessToken,
        CancellationToken cancellationToken
    )
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.googleapis.com/oauth2/v2/userinfo"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Google userinfo request failed. Status: {StatusCode}, Content: {Content}",
                response.StatusCode,
                errorContent
            );
        }

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

        var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(
            jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (userInfo == null)
        {
            _logger.LogError("Failed to deserialize Google user info response");
            throw new System.ApplicationException(
                "Failed to deserialize Google user info response"
            );
        }

        if (string.IsNullOrEmpty(userInfo.Email))
        {
            _logger.LogError("Google user info Email field is null or empty after deserialization");
            throw new System.ApplicationException(
                "Google user info Email field is required but was null or empty"
            );
        }

        return userInfo;
    }

    private async Task<DateTime?> GetGoogleBirthday(
        string accessToken,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://people.googleapis.com/v1/people/me?personFields=birthdays"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            var birthdayData = JsonSerializer.Deserialize<GooglePeopleBirthday>(
                jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (birthdayData?.Birthdays?.Any() == true)
            {
                var birthday = birthdayData.Birthdays.First().Date;
                if (
                    birthday?.Year.HasValue == true
                    && birthday.Month.HasValue
                    && birthday.Day.HasValue
                )
                {
                    var birthdayDate = new DateTime(
                        birthday.Year.Value,
                        birthday.Month.Value,
                        birthday.Day.Value
                    );
                    return birthdayDate;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get birthday from Google People API");
            return null;
        }
    }

    private string ExtractSubFromIdToken(string idToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(idToken);

            var sub = jsonToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(sub))
            {
                _logger.LogError("Sub claim not found in id_token");
                throw new System.ApplicationException("Sub claim not found in id_token");
            }

            return sub;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract Sub from id_token");
            throw new System.ApplicationException("Failed to extract Sub from id_token", ex);
        }
    }

    private class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
    }

    private class GoogleUserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
    }

    private class GooglePeopleBirthday
    {
        [JsonPropertyName("birthdays")]
        public List<BirthdayInfo>? Birthdays { get; set; }
    }

    private class BirthdayInfo
    {
        [JsonPropertyName("date")]
        public BirthdayDate? Date { get; set; }
    }

    private class BirthdayDate
    {
        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("month")]
        public int? Month { get; set; }

        [JsonPropertyName("day")]
        public int? Day { get; set; }
    }
}
