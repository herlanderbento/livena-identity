using MediatR;
using Livena.Identity.Application.UseCases.User.Common;

namespace Livena.Identity.Application.UseCases.User.UpdateUser;

public interface IUpdateUser : IRequestHandler<UpdateUserInput, UserOutput>
{
}