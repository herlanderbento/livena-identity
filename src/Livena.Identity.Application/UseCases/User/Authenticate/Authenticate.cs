using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.Authenticate;

public class Authenticate : IAuthenticate
{
    private readonly IUserRepository _userRepository;
    private readonly ICryptography _cryptography;
    private readonly IKeycloakService _keycloakService;

    public Authenticate(
        IUserRepository userRepository,
        ICryptography cryptography,
        IKeycloakService keycloakService
    )
    {
        _userRepository = userRepository;
        _cryptography = cryptography;
        _keycloakService = keycloakService;
    }

    public async Task<AuthenticateOutput> Handle(
        AuthenticateInput input,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByUsername(input.Username, cancellationToken);

        WrongCredentialsException.ThrowIfNull(user, "username or password incorrect");

        var isPasswordValid = await _cryptography.Verify(
            input.Password,
            user.Password,
            cancellationToken
        );

        WrongCredentialsException.ThrowIfFalse(isPasswordValid, "username or password incorrect");

        var token = await _keycloakService.Login(input.Username, input.Password, cancellationToken);

        return new AuthenticateOutput(
            token.AccessToken,
            token.RefreshToken,
            user: UserOutput.FromUser(user)
        );
    }
}
