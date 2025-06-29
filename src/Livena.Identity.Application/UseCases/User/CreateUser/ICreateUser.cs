using Livena.Identity.Application.UseCases.User.Common;
using MediatR;
namespace Livena.Identity.Application.UseCases.User.CreateUser;

public interface ICreateUser: IRequestHandler<CreateUserInput, UserOutput>
{
    
}