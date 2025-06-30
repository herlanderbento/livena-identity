using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.Interfaces;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.DeleteUser;

public class DeleteUser : IDeleteUser
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;
    
    public DeleteUser(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _keycloakService = keycloakService;
    }
    
    public async Task Handle(DeleteUserInput input, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(input.Id, cancellationToken);
        
        NotFoundException.ThrowIfNull(user, $"User with ID {input.Id} not found.");
        
        var keycloakId = await _keycloakService.GetKeycloakIdByExternalId(
            user.Id.ToString(), 
            cancellationToken);
        
        await _userRepository.Delete(user, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        
        if (keycloakId != null)
        {
            await _keycloakService.Delete(keycloakId, cancellationToken);
        }
    }
}