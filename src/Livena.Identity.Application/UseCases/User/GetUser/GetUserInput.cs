using Livena.Identity.Application.UseCases.User.Common;
using MediatR;

namespace Livena.Identity.Application.UseCases.User.GetUser;

public class GetUserInput(Guid id) : IRequest<UserOutput>
{
    public Guid Id { get; set; } = id;
}
