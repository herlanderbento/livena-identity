using Livena.Identity.Application.UseCases.User.Common;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.UpdateUser;

public interface IUpdateUser : IRequestHandler<UpdateUserInput, UserOutput> { }
