using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Repository;
using DomainEntity = Livena.Identity.Domain.Entity;

namespace Livena.Identity.Application.UseCases.User.CreateUser;

public class CreateUser : ICreateUser
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptography _cryptography;
    private readonly IKeycloakService _keycloakService;

    public CreateUser(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICryptography cryptography,
        IKeycloakService keycloakService
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cryptography = cryptography;
        _keycloakService = keycloakService;
    }

    public async Task<UserOutput> Handle(CreateUserInput input, CancellationToken cancellationToken)
    {
        var userWithUsername = await _userRepository.GetByUsername(
            input.Username,
            cancellationToken
        );

        ConflictException.ThrowIfNotNull(
            userWithUsername,
            $"Username '{input.Username}' is already taken."
        );

        if (!string.IsNullOrWhiteSpace(input.Email))
        {
            var userWithSameEmail = await _userRepository.GetByEmail(
                input.Email,
                cancellationToken
            );
            ConflictException.ThrowIfNotNull(
                userWithSameEmail,
                $"Email '{input.Email}' is already registered."
            );
        }

        if (!string.IsNullOrWhiteSpace(input.Phone))
        {
            var userWithSamePhone = await _userRepository.GetByPhone(
                input.Phone,
                cancellationToken
            );
            ConflictException.ThrowIfNotNull(
                userWithSamePhone,
                $"Phone '{input.Phone}' is already registered."
            );
        }

        var hashedPassword = await _cryptography.HashPassword(input.Password, cancellationToken);

        var entity = new DomainEntity.User(
            input.Username,
            input.Email,
            input.Phone,
            hashedPassword,
            input.Birthday
        );

        await _userRepository.Insert(entity, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        await _keycloakService.Insert(entity, input.Password, cancellationToken);

        return UserOutput.FromUser(entity);
    }
}
