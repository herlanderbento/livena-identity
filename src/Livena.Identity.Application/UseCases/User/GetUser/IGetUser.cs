using MediatR;
using Livena.Identity.Application.UseCases.User.Common;

namespace Livena.Identity.Application.UseCases.User.GetUser;

public interface IGetUser : IRequestHandler<GetUserInput, UserOutput>;