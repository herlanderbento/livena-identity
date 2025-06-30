using MediatR;
namespace Livena.Identity.Application.UseCases.User.DeleteUser;

public class DeleteUserInput(Guid id) : IRequest
{
    public Guid Id { get; set; } = id;
}