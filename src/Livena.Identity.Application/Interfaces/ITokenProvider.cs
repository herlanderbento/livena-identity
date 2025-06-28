using Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.Interfaces;

public interface ITokenProvider
{
    //string GenerateToken(object payload);
    string GenerateToken(User payload);

    string GenerateRefreshToken();
    bool ValidateToken(string token);
}