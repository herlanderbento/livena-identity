using Livena.Identity.Application.UseCases.User.Common;

namespace Livena.Identity.Application.UseCases.User.Authenticate;

public class AuthenticateOutput
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserOutput User { get; set; }

    public AuthenticateOutput(string accessToken, string refreshToken, UserOutput user)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        User = user;
    }
}
