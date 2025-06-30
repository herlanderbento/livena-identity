using Livena.Identity.Application.Exceptions;
using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Repository;

namespace Livena.Identity.Application.UseCases.User.GetUser;

public class GetUser: IGetUser
{
    private readonly IUserRepository _userRepository;
    
    public GetUser(IUserRepository userRepository)
        => _userRepository = userRepository;

    public async Task<UserOutput> Handle(
        GetUserInput input, 
        CancellationToken cancellationToken
    )
    {
        var user = await  _userRepository.GetById(input.Id, cancellationToken);
        
        NotFoundException.ThrowIfNull(user, $"User '{input.Id}' not found");
        
        return UserOutput.FromUser(user);
    }
}