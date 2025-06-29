using MediatR;

namespace Livena.Identity.Application.UseCases.User.Authenticate;

public class AuthenticateInput : IRequest<AuthenticateOutput>
{
    public string Username { get;  set; }
    public string Password { get;  set; }
    
    public AuthenticateInput(string username, string password)
    {
        Username = username;
        Password = password;
    }
}