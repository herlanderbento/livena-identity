using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Authenticate;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Repository;
using DomainEntity = Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.UseCases.User.GoogleAuth;

public class GoogleAuth : IGoogleAuth
{
    private readonly IOAuthService _oAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptography _cryptography;

    public GoogleAuth(
        IOAuthService oAuthService,
        IUserRepository userRepository,
        IKeycloakService keycloakService,
        IUnitOfWork unitOfWork,
        ICryptography cryptography
    )
    {
        _oAuthService = oAuthService;
        _userRepository = userRepository;
        _keycloakService = keycloakService;
        _unitOfWork = unitOfWork;
        _cryptography = cryptography;
    }

    public async Task<AuthenticateOutput> Handle(
        GoogleAuthInput input,
        CancellationToken cancellationToken
    )
    {
        var oAuthUserInfo = await _oAuthService.GetUserInfo(
            input.AuthorizationCode,
            input.RedirectUri,
            cancellationToken
        );

        KeycloakTokenResponse keycloakTokens;

        try
        {
            var userAlreadyExists = await _oAuthService.FindUserByOAuthAccount(
                oAuthUserInfo.Provider,
                oAuthUserInfo.ProviderUserId,
                cancellationToken
            );

            var oAuthAccount = await _oAuthService.GetOAuthAccount(
                userAlreadyExists.Id,
                oAuthUserInfo.Provider,
                cancellationToken
            );

            if (oAuthAccount != null)
            {
                await _oAuthService.UpdateOAuthAccount(
                    oAuthAccount,
                    oAuthUserInfo,
                    cancellationToken
                );
            }

            keycloakTokens = await _keycloakService.Login(
                userAlreadyExists.Username,
                "livena",
                cancellationToken
            );

            return new AuthenticateOutput(
                keycloakTokens.AccessToken,
                keycloakTokens.RefreshToken,
                user: UserOutput.FromUser(userAlreadyExists)
            );
        }
        catch (NotFoundException) { }

        var username = GenerateUsername(oAuthUserInfo);

        var userWithSameUsername = await _userRepository.GetByUsername(username, cancellationToken);

        ConflictException.ThrowIfNotNull(
            userWithSameUsername,
            $"Username '{username}' is already taken."
        );

        var hashedPassword = await _cryptography.HashPassword("livena", cancellationToken);

        var user = new DomainEntity.User(
            username,
            oAuthUserInfo.Email,
            null,
            hashedPassword,
            null,
            Roles.User,
            isVerified: true,
            isActive: true
        );

        await _userRepository.Insert(user, cancellationToken);

        await _keycloakService.Insert(user, "livena", cancellationToken);

        await _oAuthService.CreateOAuthAccount(user.Id, oAuthUserInfo, cancellationToken);

        await _unitOfWork.Commit(cancellationToken);

        keycloakTokens = await _keycloakService.Login(user.Username, "livena", cancellationToken);

        return new AuthenticateOutput(
            keycloakTokens.AccessToken,
            keycloakTokens.RefreshToken,
            user: UserOutput.FromUser(user)
        );
    }

    private static string GenerateUsername(OAuthUserInfo userInfo)
    {
        if (!string.IsNullOrEmpty(userInfo.Email))
        {
            var emailUsername = userInfo.Email.Split('@')[0];
            return emailUsername.Replace(".", "").Replace("+", "").ToLowerInvariant();
        }

        return $"google_{userInfo.ProviderUserId}";
    }
}
