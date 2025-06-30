using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.UpdateUser;

public class UpdateUser : IUpdateUser
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;
    
    public UpdateUser(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService
        )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _keycloakService = keycloakService;
    }
    
    public async Task<UserOutput> Handle(UpdateUserInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(input.Id, cancellationToken);
        
        NotFoundException.ThrowIfNull(user, $"User with ID {input.Id} not found.");
        
        if (!string.IsNullOrWhiteSpace(input.Email) && input.Email != user.Email)
        {
            var userWithSameEmail = await _userRepository.GetByEmail(input.Email, cancellationToken);
            ConflictException.ThrowIfNotNull(userWithSameEmail, $"Email '{input.Email}' is already registered.");
        }
        
        if (!string.IsNullOrWhiteSpace(input.Phone) && input.Phone != user.Phone)
        {
            var userWithSamePhone = await _userRepository.GetByPhone(input.Phone, cancellationToken);
            ConflictException.ThrowIfNotNull(userWithSamePhone, $"Phone '{input.Phone}' is already registered.");
        }
        
        user.Update(
            input.Email,
            input.Phone,
            input.Birthday,
            input.IsActive
        );
        
        await _userRepository.Update(user, cancellationToken);
        await _keycloakService.Update(user, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return UserOutput.FromUser(user);
    }
} 

