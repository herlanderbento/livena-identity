using Livena.Identity.Application.UseCases.User.Common;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.GetUser;

public interface IGetUser : IRequestHandler<GetUserInput, UserOutput>;
